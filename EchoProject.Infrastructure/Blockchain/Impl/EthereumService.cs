using EchoProject.Domain.Common;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;
using EchoProject.Infrastructure.Blockchain.Contracts;
using Microsoft.Extensions.Logging;

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

        public async Task<long> GetBalanceAsync(string address)
        {
            var balance = await _web3.Eth.GetBalance.SendRequestAsync(address);
            return Web3.Convert.FromWei(balance.Value).ToLong();
        }

        public async Task<bool> VerifyTransactionAsync(string txHash, string expectedContractAddress, long expectedAmount)
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
                var expectedAmountInWei = Web3.Convert.ToWei(expectedAmount);
                bool isCorrectAmount = transaction.Value.Value >= expectedAmountInWei;

                return isCorrectAddress && isCorrectAmount;
            }
            catch (Exception)
            {
                // Em caso de falha de rede ou hash inválido, negamos a verificação
                return false;
            }
        }

        public async Task<string> ReleaseFundsToSupplierAsync(string projectAddress, string supplierWallet, long amount)
        {
            // A ABI (Application Binary Interface) ensina o C# como "conversar" com o método do Smart Contract.
            // Aqui mapeamos apenas a função releaseFunds que desenhamos no Solidity.
            const string abi = @"[{""constant"":false,""inputs"":[{""name"":""_supplier"",""type"":""address""},{""name"":""_amount"",""type"":""uint256""}],""name"":""releaseFunds"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";

            // Instancia a referência do contrato no blockchain
            var contract = _web3.Eth.GetContract(abi, projectAddress);
            var releaseFunction = contract.GetFunction("releaseFunds");

            // O contrato inteligente espera receber o valor em Wei, não em Ether inteiro
            var amountInWei = Web3.Convert.ToWei(amount);

            // Configura o limite de Gás (combustível da rede) para a operação não falhar no meio
            var gasLimit = new HexBigInteger(200000);
            var valueToSend = new HexBigInteger(0); // A chamada da função em si não envia novos Ethers, apenas executa a lógica

            // Dispara a transação. O Nethereum assina automaticamente usando a conta vinculada no construtor (_web3).
            var transactionHash = await releaseFunction.SendTransactionAsync(
                _settings.EthereumAccountAddress, // De: A carteira da aplicação (Admin)
                gasLimit,
                valueToSend,
                supplierWallet, // Argumento 1 do Solidity: _supplier
                amountInWei     // Argumento 2 do Solidity: _amount
            );

            return transactionHash;
        }

        public async Task<string> DeployProjectContractAsync()
        {
            // 1. Instanciamos a nossa classe específica de deployment
            var deploymentMessage = new EchoEscrowDeployment()
            {
                PlatformAdmin = _settings.EthereumAccountAddress, // O administrador do contrato é a carteira da aplicação
                Gas = new HexBigInteger(1500000),
                FromAddress = _settings.EthereumAccountAddress
            };

            // 2. O Handler agora usa a nossa classe concreta
            var deploymentHandler = _web3.Eth.GetContractDeploymentHandler<EchoEscrowDeployment>();

            // 3. Enviamos e aguardamos a mineração do bloco
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);

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

                // Em vez de SendTransactionAsync + GetTransactionReceipt
                var receipt = await cancelFunction.SendTransactionAndWaitForReceiptAsync(
                    _settings.EthereumAccountAddress,
                    gasLimit,
                    valueToSend,
                    null // CancellationToken
                );

                return receipt != null && receipt.Status.Value == 1;
            }
            catch (Nethereum.JsonRpc.Client.RpcResponseException rpcEx)
            {
                // Isso vai te mostrar a mensagem do 'require' do Solidity (ex: "Only admin can cancel")
                _logger.LogError("Erro no RPC: {Message}", rpcEx.Message);
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}