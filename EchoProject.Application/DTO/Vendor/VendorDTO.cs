using AutoMapper;
using EchoProject.Domain.VendorAggregate;

namespace EchoProject.Application.DTO.Vendor
{
    [AutoMap(typeof(Domain.VendorAggregate.Vendor))]
    public class VendorDTO
    {
      public Guid Id {get;set;}
      public string TypeItemSupply {get;set;} = string.Empty;
      public string Name {get;set;} = string.Empty;
      public string TaxId {get;set;} = string.Empty;
      public VendorStatus Status {get;set;}
    }
}