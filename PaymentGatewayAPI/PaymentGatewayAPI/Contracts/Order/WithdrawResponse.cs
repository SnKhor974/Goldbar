namespace PaymentGatewayAPI.Contracts.Order
{
    /// <summary>
    /// withdraw response to frontend
    /// </summary>
    public class WithdrawResponse
    {
        public Guid OrderID { get; set; }
        public string TransactionID { get; set; } = string.Empty;
        public string WithdrawURL { get; set; } = string.Empty;
        public string msg { get; set; } = string.Empty;
        public bool result { get; set; }
    }
}
