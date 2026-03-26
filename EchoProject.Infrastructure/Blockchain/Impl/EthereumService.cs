using EchoProject.Domain.Common;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;
using EchoProject.Infrastructure.Blockchain.Contracts;
using Microsoft.Extensions.Logging;
using Nethereum.JsonRpc.Client;
using System.Text.Json;
using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Infrastructure.Blockchain.Impl
{
    public class EthereumService : IEthereumService
    {
        private readonly Web3 _web3;
        private readonly BlockChainSettings _settings;
        private readonly ILogger<EthereumService> _logger;

        public EthereumService(IOptions<BlockChainSettings> settings, ILogger<EthereumService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            var account = new Account(_settings.EthereumPrivateKey, _settings.ChainId);
            _web3 = new Web3(account, _settings.RpcUrl);
        }

        public async Task<decimal> GetBalanceAsync(string address)
        {
            var balance = await _web3.Eth.GetBalance.SendRequestAsync(address);
            return Web3.Convert.FromWei(balance.Value);
        }

        public async Task<bool> VerifyTransactionAsync(string txHash, string expectedContractAddress, decimal expectedAmountinETH)
        {
            try
            {
                // 1. Busca o recibo para saber se a transação teve sucesso (Status 1) ou falhou (Status 0)
                var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
                if (receipt == null || receipt.Status.Value != 1)
                    return false;

                // 2. Busca os dados brutos da transação para ler quem enviou, pra onde e quanto
                var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txHash);
                if (transaction == null)
                    return false;

                // 3. Valida se o destino do dinheiro foi realmente o contrato do projeto (ignorando case sensitive)
                bool isCorrectAddress = string.Equals(transaction.To, expectedContractAddress, StringComparison.OrdinalIgnoreCase);

                // 4. Valida se o valor depositado é maior ou igual ao esperado.
                // O valor no blockchain sempre trafega em Wei. Precisamos converter o expectedAmount (Ether) para Wei.
                var expectedAmountInWei = Web3.Convert.ToWei(expectedAmountinETH);
                bool isCorrectAmount = transaction.Value.Value >= expectedAmountInWei;

                return isCorrectAddress && isCorrectAmount;
            }
            catch (Exception)
            {
                // Em caso de falha de rede ou hash inválido, negamos a verificação
                return false;
            }
        }

        public async Task<string> ReleaseFundsToSupplierAsync(string projectAddress, string supplierWallet, decimal amount)
        {
            const string abi = @"[{""constant"":false,""inputs"":[{""name"":""_supplier"",""type"":""address""},{""name"":""_amount"",""type"":""uint256""}],""name"":""releaseFunds"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";

            var contract = _web3.Eth.GetContract(abi, projectAddress);
            var releaseFunction = contract.GetFunction("releaseFunds");

            var amountInWei = Web3.Convert.ToWei(amount);

            var gasLimit = new HexBigInteger(200000);
            var valueToSend = new HexBigInteger(0);

            var transactionHash = await releaseFunction.SendTransactionAsync(
                _settings.EthereumAccountAddress, // De: A carteira da aplicação (Admin)
                gasLimit,
                valueToSend,
                supplierWallet, // Argumento 1 do Solidity: _supplier
                amountInWei     // Argumento 2 do Solidity: _amount
            );

            _logger.LogInformation("Funds release transaction sent. Project: {ProjectAddress}, Supplier: {SupplierWallet}, Amount: {Amount}, TxHash: {TxHash}", projectAddress, supplierWallet, amount, transactionHash);

            return transactionHash;
        }

        public async Task<string> DeployProjectContractAsync()
        {
            var deploymentMessage = new EchoEscrowDeployment()
            {
                PlatformAdmin = _settings.EthereumAccountAddress,
                Gas = new HexBigInteger(1500000),
                FromAddress = _settings.EthereumAccountAddress
            };

            _logger.LogInformation("Deploying new project smart contract: {DeploymentMessage}", JsonSerializer.Serialize(deploymentMessage));

            // 2. O Handler agora usa a nossa classe concreta
            var deploymentHandler = _web3.Eth.GetContractDeploymentHandler<EchoEscrowDeployment>();

            // 3. Enviamos e aguardamos a mineração do bloco
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);

            _logger.LogInformation("Project contract deployed at address: {ContractAddress}", transactionReceipt.ContractAddress);

            return transactionReceipt.ContractAddress;
        }

        public async Task<bool> CancelSmartContractAsync(string projectAddress)
        {
            try
            {
                const string abi = @"[{""constant"":false,""inputs"":[],""name"":""cancelProject"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";

                var contract = _web3.Eth.GetContract(abi, projectAddress);
                var cancelFunction = contract.GetFunction("cancelProject");

                var gasLimit = new HexBigInteger(150000);
                var valueToSend = new HexBigInteger(0);

                var txHash = await cancelFunction.SendTransactionAsync(
                    _settings.EthereumAccountAddress,
                    gasLimit,
                    valueToSend
                );

                var receipt = await cancelFunction.SendTransactionAndWaitForReceiptAsync(
                    _settings.EthereumAccountAddress,
                    gasLimit,
                    valueToSend,
                    CancellationToken.None
                );

                _logger.LogInformation("Cancel transaction sent for project {ProjectAddress}. Transaction hash: {TxHash}", projectAddress, txHash);
                _logger.LogInformation("Receipt: {Receipt}", JsonSerializer.Serialize(receipt));

                return receipt != null && receipt.Status.Value == 1;
            }
            catch (RpcResponseException rpcEx)
            {
                _logger.LogError("RPC errof for project {ProjectAddress}: {Message}", projectAddress, rpcEx.Message);
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DonationStatus> GetDonationStatus(
            string transactionId,
            string expectedReceivingVendorWallet, 
            decimal expectedAmountInETH,
            bool isMoneyDonation)
        {
            var pendingStatus = isMoneyDonation ? DonationStatus.ImmediateTransferToNGOPending : DonationStatus.TransferredToVendorPending;
            var confirmedStatus = isMoneyDonation ? DonationStatus.ImmediateTransferToNGOConfirmed : DonationStatus.TransferredToVendorConfirmed;

            try
            {    
                var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionId);

                if (receipt == null) return pendingStatus; 

                if (receipt.Status.Value == 0) return DonationStatus.Failed;

                var releaseEvent = receipt.DecodeAllEvents<FundsReleasedEvent>();

                var validLog = releaseEvent.FirstOrDefault(log =>
                    string.Equals(log.Event.Vendor, expectedReceivingVendorWallet, StringComparison.OrdinalIgnoreCase) &&
                    Web3.Convert.FromWei(log.Event.Amount) == expectedAmountInETH
                );

                if (validLog != null)
                {
                    return confirmedStatus;
                }

                _logger.LogWarning("Transação {Hash} confirmada, mas logs não batem com esperado!", transactionId);
                return DonationStatus.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na auditoria de logs da transação {Hash}", transactionId);
                return pendingStatus;
            }
        }

    }
}