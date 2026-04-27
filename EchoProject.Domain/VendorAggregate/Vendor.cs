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
                throw new ArgumentException("O nome não pode estar vazio.");

            Name = name;
            Document = document.IsCnpj ? document : throw new ArgumentException("Fornecedores devem ser cadastrados como empresa (CNPJ).");
            Wallet = wallet;
            TypeItemSupply = typeItemSupply;
        }

        public void Approve(Guid adminId)
        {
            if (Status == VendorStatus.Approved)
                throw new DomainException("O fornecedor já foi aprovado.");

            if (Status == VendorStatus.Rejected)
                throw new DomainException("Não é possível aprovar um fornecedor rejeitado. Revise as informações e envie uma nova solicitação.");

            if (Status == VendorStatus.Disabled)
                throw new DomainException("Não é possível aprovar um fornecedor desativado. Revise as informações e envie uma nova solicitação.");

            if (Status != VendorStatus.Pending)
                throw new DomainException("Apenas fornecedores pendentes podem ser aprovados.");

            Status = VendorStatus.Approved;
            ApprovedById = adminId;
            DecisionDate = DateTime.UtcNow;
        }

        public void Deny(Guid adminId)
        {
            if (Status == VendorStatus.Rejected)
                throw new DomainException("O fornecedor já foi rejeitado.");
            
            if (Status == VendorStatus.Approved)
                throw new DomainException("Não é possível rejeitar um fornecedor aprovado. Revise as informações e envie uma nova solicitação.");
            
            if (Status == VendorStatus.Disabled)
                throw new DomainException("Não é possível rejeitar um fornecedor desativado. Revise as informações e envie uma nova solicitação.");
            
            if (Status != VendorStatus.Pending)
                throw new DomainException("Apenas fornecedores pendentes podem ser rejeitados.");

            Status = VendorStatus.Rejected;
            ApprovedById = adminId;
            DecisionDate = DateTime.UtcNow;
        }

        public void Disable()
        {
            if (Status == VendorStatus.Disabled)
                throw new DomainException("O fornecedor já está desativado.");
            
            if (Status != VendorStatus.Approved)
                throw new DomainException("Apenas fornecedores aprovados podem ser desativados.");
                
            Status = VendorStatus.Disabled;
            DecisionDate = DateTime.UtcNow;
        }

        public void Reavaluate()
        {
            if (DecisionDate is not null && DecisionDate.Value.AddDays(30) > DateTime.UtcNow)
                throw new DomainException("Fornecedores só podem ser reavaliados 30 dias após a última decisão.");
                
            if (Status != VendorStatus.Disabled)
                throw new DomainException("Apenas fornecedores desativados podem ser reenviados para reavaliação.");

            Status = VendorStatus.Pending;
            DecisionDate = DateTime.UtcNow;
        }

        public bool IsValid()
        {
            return Status == VendorStatus.Approved;
        }
    }
}
