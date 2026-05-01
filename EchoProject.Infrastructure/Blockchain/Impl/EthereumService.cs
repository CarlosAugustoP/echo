using System.Diagnostics;
using System.Text.Json;
using EchoProject.Domain.Common;
using EchoProject.Domain.DonationAggregate;
using EchoProject.Infrastructure.Blockchain.Contracts;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace EchoProject.Infrastructure.Blockchain.Impl
{
    public class EthereumService : IEthereumService
    {
        private static readonly TimeSpan ReceiptPollingInterval = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan ReceiptPollingTimeout = TimeSpan.FromSeconds(90);
        private static readonly TimeSpan ContractCodePollingTimeout = TimeSpan.FromSeconds(30);

        private readonly Web3 _web3;
        private readonly BlockChainSettings _settings;
        private readonly ILogger<EthereumService> _logger;
        private readonly string _accountAddress;

        public EthereumService(IOptions<BlockChainSettings> settings, ILogger<EthereumService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            var account = new Account(_settings.EthereumPrivateKey, _settings.ChainId);
            _accountAddress = account.Address;
            _web3 = new Web3(account, _settings.RpcUrl);

            if (!string.IsNullOrWhiteSpace(_settings.EthereumAccountAddress) &&
                !string.Equals(_settings.EthereumAccountAddress, _accountAddress, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Configured Ethereum account does not match the private key derived address. Configured: {ConfiguredAddress}, Derived: {DerivedAddress}",
                    _settings.EthereumAccountAddress,
                    _accountAddress);
            }
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
                var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
                if (receipt == null || receipt.Status.Value != 1)
                    return false;

                var transaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txHash);
                if (transaction == null)
                    return false;

                bool isCorrectAddress = string.Equals(transaction.To, expectedContractAddress, StringComparison.OrdinalIgnoreCase);
                var expectedAmountInWei = Web3.Convert.ToWei(expectedAmountinETH);
                bool isCorrectAmount = transaction.Value.Value >= expectedAmountInWei;

                return isCorrectAddress && isCorrectAmount;
            }
            catch (Exception)
            {
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
                _accountAddress,
                gasLimit,
                valueToSend,
                supplierWallet,
                amountInWei
            );

            _logger.LogInformation(
                "Funds release transaction sent. Project: {ProjectAddress}, Supplier: {SupplierWallet}, Amount: {Amount}, TxHash: {TxHash}",
                projectAddress,
                supplierWallet,
                amount,
                transactionHash);

            return transactionHash;
        }

        public async Task<string> DeployProjectContractAsync()
        {
            string? txHash = null;
            TransactionReceipt? receipt = null;

            var deploymentMessage = new EchoEscrowDeployment
            {
                PlatformAdmin = _accountAddress,
                Gas = new HexBigInteger(1500000),
                FromAddress = _accountAddress
            };

            _logger.LogInformation(
                "Starting smart contract deployment. From: {FromAddress}, PlatformAdmin: {PlatformAdmin}, ChainId: {ChainId}, Gas: {Gas}, RpcUrl: {RpcUrl}",
                deploymentMessage.FromAddress,
                deploymentMessage.PlatformAdmin,
                _settings.ChainId,
                deploymentMessage.Gas,
                _settings.RpcUrl);

            try
            {
                var deploymentHandler = _web3.Eth.GetContractDeploymentHandler<EchoEscrowDeployment>();
                txHash = await deploymentHandler.SendRequestAsync(deploymentMessage);

                _logger.LogInformation("Deployment transaction submitted. TxHash: {TxHash}", txHash);

                var sentTransaction = await _web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txHash);
                if (sentTransaction != null)
                {
                    _logger.LogInformation(
                        "Deployment transaction details. TxHash: {TxHash}, Nonce: {Nonce}, Gas: {Gas}, GasPrice: {GasPrice}, MaxFeePerGas: {MaxFeePerGas}, MaxPriorityFeePerGas: {MaxPriorityFeePerGas}",
                        txHash,
                        sentTransaction.Nonce?.Value,
                        sentTransaction.Gas?.Value,
                        sentTransaction.GasPrice?.Value,
                        sentTransaction.MaxFeePerGas?.Value,
                        sentTransaction.MaxPriorityFeePerGas?.Value);
                }
                else
                {
                    _logger.LogWarning("Deployment transaction was submitted but could not be fetched immediately. TxHash: {TxHash}", txHash);
                }

                receipt = await WaitForReceiptAsync(txHash, ReceiptPollingTimeout);
                if (receipt == null)
                {
                    var latestBlock = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                    var balance = await _web3.Eth.GetBalance.SendRequestAsync(_accountAddress);

                    _logger.LogError(
                        "Deployment receipt timeout. TxHash: {TxHash}, WaitedSeconds: {WaitedSeconds}, LatestBlock: {LatestBlock}, SenderBalanceWei: {SenderBalanceWei}",
                        txHash,
                        ReceiptPollingTimeout.TotalSeconds,
                        latestBlock.Value,
                        balance.Value);

                    throw new TimeoutException($"Timed out waiting for deployment receipt. TxHash: {txHash}");
                }

                _logger.LogInformation(
                    "Deployment receipt received. TxHash: {TxHash}, Status: {Status}, ContractAddress: {ContractAddress}, GasUsed: {GasUsed}, CumulativeGasUsed: {CumulativeGasUsed}, EffectiveGasPrice: {EffectiveGasPrice}, BlockNumber: {BlockNumber}",
                    txHash,
                    receipt.Status?.Value,
                    receipt.ContractAddress ?? "<null>",
                    receipt.GasUsed?.Value,
                    receipt.CumulativeGasUsed?.Value,
                    receipt.EffectiveGasPrice?.Value,
                    receipt.BlockNumber?.Value);

                if (receipt.Status?.Value == 0)
                {
                    throw new InvalidOperationException(
                        $"Deployment transaction mined with failure status. TxHash: {txHash}, GasUsed: {receipt.GasUsed?.Value}");
                }

                if (string.IsNullOrWhiteSpace(receipt.ContractAddress))
                {
                    throw new InvalidOperationException(
                        $"Deployment receipt did not include a contract address. TxHash: {txHash}");
                }

                var hasCode = await WaitForContractCodeAsync(receipt.ContractAddress, ContractCodePollingTimeout);
                if (!hasCode)
                {
                    _logger.LogError(
                        "Deployment receipt succeeded but contract code was not found in time. TxHash: {TxHash}, ContractAddress: {ContractAddress}",
                        txHash,
                        receipt.ContractAddress);

                    throw new InvalidOperationException(
                        $"Deployment receipt succeeded but contract code was not found. TxHash: {txHash}, ContractAddress: {receipt.ContractAddress}");
                }

                _logger.LogInformation(
                    "Project contract deployed successfully. TxHash: {TxHash}, ContractAddress: {ContractAddress}",
                    txHash,
                    receipt.ContractAddress);

                return receipt.ContractAddress;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Smart contract deployment failed. TxHash: {TxHash}, ReceiptStatus: {ReceiptStatus}, ReceiptContractAddress: {ReceiptContractAddress}, ReceiptGasUsed: {ReceiptGasUsed}, DeploymentMessage: {DeploymentMessage}",
                    txHash ?? "<not-submitted>",
                    receipt?.Status?.Value,
                    receipt?.ContractAddress ?? "<null>",
                    receipt?.GasUsed?.Value,
                    JsonSerializer.Serialize(deploymentMessage));

                throw;
            }
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
                    _accountAddress,
                    gasLimit,
                    valueToSend
                );

                var receipt = await cancelFunction.SendTransactionAndWaitForReceiptAsync(
                    _accountAddress,
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
            var pendingStatus = isMoneyDonation ? DonationStatus.ImmediateTransferToNGOInContract : DonationStatus.TransferredToVendorPending;
            var confirmedStatus = isMoneyDonation ? DonationStatus.ImmediateTransferToNGOConfirmed : DonationStatus.TransferredToVendorConfirmed;

            try
            {
                var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionId);

                if (receipt == null) return pendingStatus;

                if (receipt.Status.Value == 0) return DonationStatus.Failed;

                var releaseEvent = receipt.DecodeAllEvents<FundsReleasedEvent>();

                var validLog = releaseEvent.FirstOrDefault(log =>
                {
                    var vendor = log.Event.Supplier;
                    var amount = Web3.Convert.FromWei(log.Event.Amount);
                    var vendorMatch = string.Equals(vendor, expectedReceivingVendorWallet, StringComparison.OrdinalIgnoreCase);
                    var amountMatch = amount == expectedAmountInETH;

                    _logger.LogInformation(
                        "Event log check - Vendor: {ActualVendor} (Expected: {ExpectedVendor}) Match: {VendorMatch}, Amount: {ActualAmount} (Expected: {ExpectedAmount}) Match: {AmountMatch}",
                        vendor, expectedReceivingVendorWallet, vendorMatch,
                        amount, expectedAmountInETH, amountMatch
                    );

                    return vendorMatch && amountMatch;
                });

                if (validLog != null)
                {
                    return confirmedStatus;
                }

                _logger.LogWarning("Transacao {Hash} confirmada, mas logs nao batem com esperado!", transactionId);
                return DonationStatus.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na auditoria de logs da transacao {Hash}", transactionId);
                return pendingStatus;
            }
        }

        public void ValidateEthereumWallet(string walletAddress)
        {
            if (string.IsNullOrWhiteSpace(walletAddress))
            {
                throw new FormatException("O endereco da carteira Ethereum nao pode estar vazio.");
            }

            var addressUtil = new AddressUtil();

            if (!addressUtil.IsValidEthereumAddressHexFormat(walletAddress))
            {
                throw new FormatException($"O endereco '{walletAddress}' nao possui um formato Ethereum valido.");
            }

            if (!addressUtil.IsChecksumAddress(walletAddress))
            {
                throw new FormatException($"O endereco '{walletAddress}' e invalido. Falha na validacao de checksum.");
            }
        }

        private async Task<TransactionReceipt?> WaitForReceiptAsync(string txHash, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < timeout)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txHash);
                if (receipt != null)
                {
                    return receipt;
                }

                await Task.Delay(ReceiptPollingInterval, cancellationToken);
            }

            return null;
        }

        private async Task<bool> WaitForContractCodeAsync(string contractAddress, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < timeout)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var code = await _web3.Eth.GetCode.SendRequestAsync(contractAddress);
                if (!string.IsNullOrWhiteSpace(code) && !string.Equals(code, "0x", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation(
                        "Contract code detected. ContractAddress: {ContractAddress}, CodeLength: {CodeLength}",
                        contractAddress,
                        code.Length);

                    return true;
                }

                await Task.Delay(ReceiptPollingInterval, cancellationToken);
            }

            return false;
        }
        
    }
}
