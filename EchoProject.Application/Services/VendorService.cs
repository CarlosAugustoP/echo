using EchoProject.Application.Common;
using EchoProject.Application.DTO;
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
            var vendor = await _unitOfWork..FindByIdAsync(vendorId);
    }
}