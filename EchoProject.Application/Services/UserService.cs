using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.Common.Auth;
using EchoProject.Application.Common.Password;
using EchoProject.Application.DTO;
using EchoProject.Application.Exceptions;
using EchoProject.Application.Requests.Login;
using EchoProject.Application.Requests.Signup;
using EchoProject.Application.Requests.Users;
using EchoProject.Domain.Common;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.ValueObjects;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using EchoProject.Infrastructure.Storage.Client;
using Microsoft.Extensions.Logging;

namespace EchoProject.Application.Services
{
    [AppService]
    public class UserService
    {
        private readonly IEthereumService _ethereumService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserService> _logger;
        private readonly IJwtService _jwt;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageClient _storage;
        private readonly IMapper _mapper;

        public UserService(
            IEthereumService ethereumService,
            IPasswordHasher passwordHasher,
            ILogger<UserService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper, 
            IJwtService jwt,
            IStorageClient storage)
        {
            _ethereumService = ethereumService;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _jwt = jwt;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _storage = storage;
        }

        public async Task<UserDTO> RegisterUserAsync(SignupRequest request)
        {
            var existingUser = await _unitOfWork.Users.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ConflictException("Email already in use", "USER_ALREADY_EXISTS");
            }

            var hashedPassword = _passwordHasher.Hash(request.Password);
            var address = request.Address;

            try 
            {
                _ethereumService.ValidateEthereumWallet(request.WalletAddress);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            var user = new User
            (
                request.Name, 
                request.Email, 
                hashedPassword,
                new TaxId(request.TaxId), 
                new WalletAddress(request.WalletAddress), 
                new Address
                (
                    address.ZipCode,
                    address.Street, 
                    address.Neighborhood, 
                    address.City, 
                    address.State, 
                    address.CountryCode, 
                    address.Number
                ), 
                request.Role
            );

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserDTO>(user);     
        }

        public async Task<string> LoginAsync(LoginRequest req)
        {
            var user = await _unitOfWork.Users.FindByEmailAsync(req.Email);

            if (user == null || !_passwordHasher.Validate(req.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password", "INVALID_CREDENTIALS");
            }

            var userDto = _mapper.Map<UserDTO>(user);
        
            return _jwt.GenerateToken(userDto);
        }

        public long GetEchos(UserDTO user)
        { 
            decimal echoAmount = 0;
            if (user.Role == UserRole.Donor)
            {
                var allDonations = _unitOfWork.Donations
                    .FindAll(d => d.DonorId == user.Id);

                echoAmount = allDonations
                    .Where(d => d.Status == DonationStatus.ImmediateTransferToNGOConfirmed 
                        || d.Status == DonationStatus.TransferredToVendorConfirmed)
                    .Sum(d => d.Amount * d.TotalCost);
            }
            else if (user.Role == UserRole.NGO)
            {
                var allReceivedDonations = _unitOfWork.Donations
                    .FindAll(d => d.Goal.Project.ManagerId == user.Id);
                
                echoAmount = allReceivedDonations
                    .Where(d => d.Status == DonationStatus.ImmediateTransferToNGOConfirmed 
                        || d.Status == DonationStatus.TransferredToVendorConfirmed)
                    .Sum(d => d.Amount * d.TotalCost);
            }
            return (long)(echoAmount * 1000);
        } 

        public async Task<UserDTO> UpdateProfile(UpdateUserRequest request, UserDTO user)
        {
            var userEntity = await _unitOfWork.Users.FindByIdAsync(user.Id) 
                ?? throw new NotFoundException("User not found.");

            string? pfp = request.ProfilePictureBase64 != null ?
                await _storage.UploadFileAsync("profile_" + user.Id, request.ProfilePictureBase64.ToStream()) : null;

            ImageUrl? profilePictureUrl = pfp != null ? new(pfp) : null;

            userEntity.UpdateInformation
                (
                    request.Name, 
                    request.Email, 
                    request.Address != null ? new Address
                    (
                        request.Address.ZipCode,
                        request.Address.Street, 
                        request.Address.Neighborhood, 
                        request.Address.City, 
                        request.Address.State, 
                        request.Address.CountryCode, 
                        request.Address.Number
                    ) : userEntity.Address, 
                    profilePictureUrl
                );
            
            _unitOfWork.Users.Update(userEntity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<UserDTO>(userEntity);
        }

        public async Task<UserDTO> UpdateWalletAddress(Guid userId, string newWalletAddress)
        {
            var user = await _unitOfWork.Users.FindByIdAsync(userId) 
                ?? throw new NotFoundException("User not found.");

            try 
            {
                _ethereumService.ValidateEthereumWallet(newWalletAddress);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            
            user.UpdateWalletAddress(new WalletAddress(newWalletAddress));
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<UserDTO>(user);
        }
    }
}