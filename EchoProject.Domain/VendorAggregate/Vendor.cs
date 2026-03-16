using EchoProject.Domain.Common;
using EchoProject.Domain.Exception.EchoProject.Domain.Common;
using EchoProject.Domain.UserAggregate;
using EchoProject.Domain.ValueObjects;

namespace EchoProject.Domain.VendorAggregate
{
    public class Vendor : Entity
    {
        public string Name { get; private set; }
        public TaxId Document { get; private set; }
        public WalletAddress Wallet { get; private set; }
        public VendorStatus Status { get; private set; } = VendorStatus.Pending;
        public string TypeItemSupply { get; private set; }
        public Guid? ApprovedById { get; private set; }
        public User? ApprovedBy { get; private set; } = null;
        public DateTime? DecisionDate { get; private set; }
        private Vendor() { } // EF Core

        public Vendor(string? name, TaxId document, WalletAddress wallet, string typeItemSupply)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");

            Name = name;
            Document = document.IsCnpj ? document : throw new ArgumentException("Vendors must be registered as a company (CNPJ)");
            Wallet = wallet;
            TypeItemSupply = typeItemSupply;
        }

        public void Approve(Guid adminId)
        {
            if (Status == VendorStatus.Approved)
                throw new DomainException("Vendor is already approved.");

            if (Status == VendorStatus.Rejected)
                throw new DomainException("Cannot approve a rejected vendor. Please review the vendor's information and submit a new application.");

            if (Status == VendorStatus.Disabled)
                throw new DomainException("Cannot approve a disabled vendor. Please review the vendor's information and submit a new application.");

            if (Status != VendorStatus.Pending)
                throw new DomainException("Only pending vendors can be approved.");

            Status = VendorStatus.Approved;
            ApprovedById = adminId;
            DecisionDate = DateTime.UtcNow;
        }

        public void Deny(Guid adminId)
        {
            if (Status == VendorStatus.Rejected)
                throw new DomainException("Vendor is already rejected.");
            
            if (Status == VendorStatus.Approved)
                throw new DomainException("Cannot reject an approved vendor. Please review the vendor's information and submit a new application.");
            
            if (Status == VendorStatus.Disabled)
                throw new DomainException("Cannot reject a disabled vendor. Please review the vendor's information and submit a new application.");
            
            if (Status != VendorStatus.Pending)
                throw new DomainException("Only pending vendors can be rejected.");

            Status = VendorStatus.Rejected;
            ApprovedById = adminId;
            DecisionDate = DateTime.UtcNow;
        }

        public void Disable()
        {
            if (Status == VendorStatus.Disabled)
                throw new DomainException("Vendor is already disabled.");
            
            if (Status != VendorStatus.Approved)
                throw new DomainException("Only approved vendors can be disabled.");
                
            Status = VendorStatus.Disabled;
            DecisionDate = DateTime.UtcNow;
        }

        public void Reavaluate()
        {
            if (DecisionDate is not null && DecisionDate.Value.AddDays(30) > DateTime.UtcNow)
                throw new DomainException("Vendors can only be re-evaluated after 30 days from the last decision date.");
                
            if (Status != VendorStatus.Disabled)
                throw new DomainException("Only disabled vendors can be submitted again for re-evaluation.");

            Status = VendorStatus.Pending;
            DecisionDate = DateTime.UtcNow;
        }

        public bool IsValid()
        {
            return Status == VendorStatus.Approved;
        }
    }
}