using Consumer.Contracts.Enumerables;

namespace Consumer.Contracts.Deposit
{
    public class NotifyOrderRequest
    {
        /// <summary>
        /// queue content for consumer (rabbitmq)
        /// </summary>
        public EnumMerchantContracts Merchants { get; set; }
        public string orderRequest { get; set; } = string.Empty;
    }
}
