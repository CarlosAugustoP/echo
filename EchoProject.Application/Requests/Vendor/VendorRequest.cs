namespace EchoProject.Application.Requests.Vendor
{
    public record VendorRequest(string TaxId, string Name, string WalletAddress, string TypeItemSupply);
}