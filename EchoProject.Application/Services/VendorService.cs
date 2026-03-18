using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.DTO;
using EchoProject.Application.DTO.Vendor;
using EchoProject.Application.Exceptions;
using EchoProject.Application.Requests.Vendor;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.ValueObjects;
using EchoProject.Domain.VendorAggregate;

namespace EchoProject.Application.Services
{
    [AppService]
    public class VendorService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<bool> ApproveVendorAsync(Guid vendorId, UserDTO admin)
        {
            var vendor = await _unitOfWork.Vendors.FindByIdAsync(vendorId)
                ?? throw new NotFoundException($"Vendor with ID {vendorId} not found.");
            vendor.Approve(admin.Id);
            _unitOfWork.Commit();
            return true;
        }

        public async Task<bool> RejectVendorAsync(Guid vendorId, UserDTO admin)
        {
            var vendor = await _unitOfWork.Vendors.FindByIdAsync(vendorId)
                ?? throw new NotFoundException($"Vendor with ID {vendorId} not found.");
            vendor.Deny(admin.Id);
            _unitOfWork.Commit();
            return true;
        }

        public async Task<VendorDTO> CreateAsync(VendorRequest request)
        {
            var existingByTaxId = await _unitOfWork.Vendors.FindAsync(x => x.Document.Value == request.TaxId);
            
            if (existingByTaxId != null)
            {
                existingByTaxId.Reavaluate();
                return _mapper.Map<VendorDTO>(existingByTaxId);   
            }

            var vendor = new Vendor
            (
                request.Name,
                new TaxId(request.TaxId), 
                new WalletAddress(request.WalletAddress), 
                request.TypeItemSupply
            );
            await _unitOfWork.Vendors.AddAsync(vendor);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<VendorDTO>(vendor);
        }

        public async Task<VendorDTO> GetByIdAsync(Guid vendorId)
        {
            var vendor = await _unitOfWork.Vendors.FindByIdAsync(vendorId)
                ?? throw new NotFoundException($"Vendor with ID {vendorId} not found.");
            return _mapper.Map<VendorDTO>(vendor);
        }

        public async Task<bool> AssignVendorToGoalAsync(Guid vendorId, Guid goalId, UserDTO ngo)
        {
            var vendor = await _unitOfWork.Vendors.FindByIdAsync(vendorId)
                ?? throw new NotFoundException($"Vendor with ID {vendorId} not found.");
            
            var goal = await _unitOfWork.Goals.FindByIdAsync(goalId)
                ?? throw new NotFoundException($"Goal with ID {goalId} not found.");
            
            if (goal.Project.ManagerId != ngo.Id)
                throw new UnauthorizedException("Only the project manager can assign vendors to goals.");

            goal.AssignVendor(vendor);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}