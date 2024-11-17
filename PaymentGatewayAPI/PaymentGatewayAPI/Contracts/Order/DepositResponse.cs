namespace PaymentGatewayAPI.Contracts
{
    /// <summary>
    /// deposit response to frontend
    /// </summary>
    public class DepositResponse
    {
        public Guid OrderID { get; set; }
        public string TransactionID { get; set; } = string.Empty;
        public string DepositURL { get; set; } = string.Empty;
        public string msg { get; set; } = string.Empty;
        public bool result { get; set; }

    }
}
