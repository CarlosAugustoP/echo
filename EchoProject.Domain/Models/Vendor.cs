using EchoProject.Domain.Common;
using EchoProject.Domain.Enums;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Domain.Models
{
    public class Vendor : Entity
{
    public string Name { get; private set; }
    public TaxId Document { get; private set; }
    public WalletAddress Wallet { get; private set; }
    public VendorStatus Status { get; private set; } = VendorStatus.Pending;
    public Guid? ApprovedById { get; private set; }

    public Vendor(string name, TaxId document, WalletAddress wallet)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");

        Name = name;
        Document = document;
        Wallet = wallet;
    }

    public void Approve(Guid adminId)
    {
        Status = VendorStatus.Approved;
        ApprovedById = adminId;
    }

    public void Deny(Guid adminId)
    {
        Status = VendorStatus.Rejected;
        ApprovedById = adminId;
    }
    
    public void Disable(Guid adminId)
    {
        Status = VendorStatus.Disabled;
        ApprovedById = adminId;
    }
}
}