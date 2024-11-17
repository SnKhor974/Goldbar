using System;

namespace PaymentGatewayAPI.Contracts
{
    /// <summary>
    /// deposit request from frontend
    /// </summary>
    public class DepositRequest
    {
        public string MerchantName { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string WalletAddress { get; set; } = string.Empty;
        public string TransactionID { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public string DateTime { get; set; } = string.Empty;

    }
}
