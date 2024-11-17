using System;

namespace PaymentGatewayAPI.Contracts.MerchantSettings
{
    public class MerchantSettings
    {
        public string MerchantName { get; set; } = string.Empty;
        public string MerchantHost { get; set; } = string.Empty;
        public string MerchantAccountName { get; set; } = string.Empty;
        public int MerchantAccountID { get; set; }
        public string MerchantIP { get; set; } = string.Empty;
    }
}