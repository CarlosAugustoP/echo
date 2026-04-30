using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.Common.PaginatedList;
using EchoProject.Application.DTO;
using EchoProject.Application.DTO.Vendor;
using EchoProject.Application.Exceptions;
using EchoProject.Application.Requests.Pagination;
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
                ?? throw new NotFoundException($"Fornecedor com ID {vendorId} não encontrado.");
            vendor.Approve(admin.Id);
            _unitOfWork.Commit();
            return true;
        }

        public async Task<bool> RejectVendorAsync(Guid vendorId, UserDTO admin)
        {
            var vendor = await _unitOfWork.Vendors.FindByIdAsync(vendorId)
                ?? throw new NotFoundException($"Fornecedor com ID {vendorId} não encontrado.");
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

        public async Task<PaginatedList<VendorDTO>> GetAllAsync(PageRequest p)
        {
            var vendors = _unitOfWork.Vendors.FindAll();
            return vendors.Paginate(p.PageNumber, p.PageSize)
                .Select(x => _mapper.Map<VendorDTO>(x));
        }

        public async Task<VendorDTO> GetByIdAsync(Guid vendorId)
        {
            var vendor = await _unitOfWork.Vendors.FindByIdAsync(vendorId)
                ?? throw new NotFoundException($"Fornecedor com ID {vendorId} não encontrado.");
            return _mapper.Map<VendorDTO>(vendor);
        }

        public async Task<bool> AssignVendorToGoalAsync(Guid vendorId, Guid goalId, UserDTO ngo)
        {
            var vendor = await _unitOfWork.Vendors.FindByIdAsync(vendorId)
                ?? throw new NotFoundException($"Fornecedor com ID {vendorId} não encontrado.");
            
            var goal = await _unitOfWork.Goals.FindByIdAsync(goalId)
                ?? throw new NotFoundException($"Meta com ID {goalId} não encontrada.");
            
            if (goal.Project.ManagerId != ngo.Id)
                throw new UnauthorizedException("Apenas o gestor do projeto pode vincular fornecedores às metas.");

            goal.AssignVendor(vendor);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
