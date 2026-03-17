using AutoMapper;
using EchoProject.Domain.ValueObjects;
using EchoProject.Domain.VendorAggregate;

namespace EchoProject.Application.DTO.Vendor
{
    [AutoMap(typeof(Domain.VendorAggregate.Vendor))]
    public class VendorDTO
    {
      public Guid Id {get;set;}
      public string TypeItemSupply {get;set;} = string.Empty;
      public string Name {get;set;} = string.Empty;
      public TaxId Document {get;set;} = null!;
      public VendorStatus Status {get;set;}
    }
}