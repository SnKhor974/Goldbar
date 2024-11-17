namespace PaymentGatewayAPI.Contracts.TransactionDetails
{
    /// <summary>
    /// withdraw transaction details for repository
    /// </summary>
    public class WithdrawTransactionDetails
    {
        public Guid OrderID { get; set; }
        public string MerchantName { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string WalletAddress { get; set; } = string.Empty;
        public string TransactionID { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Network { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public string DateTime { get; set; } = string.Empty;
    }
}