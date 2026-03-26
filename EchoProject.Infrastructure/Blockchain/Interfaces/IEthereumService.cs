using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Infrastructure.Blockchain.Interfaces
{
    public interface IEthereumService
    {
        Task<decimal> GetBalanceAsync(string address);

        /// <summary>
        /// Verifies that a transaction with the given hash exists, was successful, and matches the expected contract address and amount.
        /// </summary>
        /// <param name="txHash"></param>
        /// <param name="expectedContractAddress"></param>
        /// <param name="expectedAmount"></param>
        /// <returns></returns>
        Task<bool> VerifyTransactionAsync(string txHash, string expectedContractAddress, decimal expectedAmountInETH);

        /// <summary>
        /// Releases funds from the contract to the supplier's wallet 
        /// (after the amount has been transferred to a verified vendor). 
        /// </summary>
        /// <param name="projectAddress"></param>
        /// <param name="supplierWallet"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        Task<string> ReleaseFundsToSupplierAsync(string projectAddress, string supplierWallet, decimal amount);
        /// <summary>
        /// Deploys a new instance of the project smart contract to the Ethereum blockchain and returns its address.
        /// </summary>
        /// <returns></returns>
        Task<string> DeployProjectContractAsync();
        Task<bool> CancelSmartContractAsync(string projectAddress); 
        Task<DonationStatus> GetDonationStatus(string transactionId, string expectedReceivingVendorWallet, decimal expectedAmountInETH, bool isMoneyDonation);
    }
}