using CallbackAPI.Contracts.Enumerables;

namespace CallbackAPI.Queue.Contract
{
    //message to send to queue
    public class NotifyOrderRequest
    {
        public EnumMerchantContracts Merchants { get; set; }
        public string orderRequest { get; set; }
    }
}
