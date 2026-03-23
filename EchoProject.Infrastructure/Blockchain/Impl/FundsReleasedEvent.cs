using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace EchoProject.Infrastructure.Blockchain.Impl
{
    [Event("FundsReleased")]
    public class FundsReleasedEvent : IEventDTO
    {
        [Parameter("address", "vendor", 1, true)]
        public string Vendor { get; set; } = string.Empty;

        [Parameter("uint256", "amount", 2, false)]
        public BigInteger Amount { get; set; }
    }
}