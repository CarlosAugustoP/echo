using EchoProject.Application.Common;
using EchoProject.Application.DTO;
using EchoProject.Application.Exceptions;
using EchoProject.Domain.Interfaces;

namespace EchoProject.Application.Services
{
    [AppService]
    public class VendorService
    {
       private readonly IUnitOfWork _unitOfWork;

        public VendorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ApproveVendorAsync(Guid vendorId, UserDTO admin)
        {
            var vendor = await _unitOfWork.Vendors.FindByIdAsync(vendorId)
                ?? throw new NotFoundException($"Vendor with ID {vendorId} not found.");
            vendor.Approve(admin.Id);
            return true;
        }

        public async Task
    }
}