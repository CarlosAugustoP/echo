using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.Common.Password;
using EchoProject.Application.DTO;
using EchoProject.Application.Exception;
using EchoProject.Application.Requests.Signup;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.ValueObjects;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EchoProject.Application.Services
{
    [AppService]
    public class UserService
    {
        private readonly IEthereumService _ethereumService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(
            IEthereumService ethereumService,
            IPasswordHasher passwordHasher,
            ILogger<UserService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _ethereumService = ethereumService;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

            var user = new User
            (
                request.Name, 
                request.Email, 
                new TaxId(request.TaxId), 
                new WalletAddress(request.WalletAddress), 
                new Address(address.ZipCode, address.Street, address.Number, address.City, address.State, address.CountryCode), 
                UserRole.Donor);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserDTO>(user);
            
        }
    }
}