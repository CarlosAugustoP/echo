using EchoProject.Application.Common.PaginatedList;

namespace EchoProject.Application.DTO.Vendor
{
    public record VendorSearchResponseDTO(PaginatedList<VendorDTO> Vendors, int TotalPending, int TotalApproved);
}