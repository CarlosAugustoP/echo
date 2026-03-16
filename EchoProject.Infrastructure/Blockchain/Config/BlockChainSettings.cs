namespace EchoProject.Infrastructure.Blockchain
{
    public class BlockChainSettings
    {
        public string RpcUrl { get; set; } = string.Empty;
        public string EthereumPrivateKey { get; set; } = string.Empty;
        public string EthereumAccountAddress { get; set; } = string.Empty;
        public int ChainId { get; set; }
    }
}