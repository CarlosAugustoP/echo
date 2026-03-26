using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace EchoProject.Infrastructure.Blockchain.Impl
{
    [Event("FundsReleased")]
    public class FundsReleasedEvent : IEventDTO
    {
        [Parameter("address", "supplier", 1, true)]
        public string Supplier { get; set; } = string.Empty;

        [Parameter("uint256", "amount", 2, false)]
        public BigInteger Amount { get; set; }
    }
}