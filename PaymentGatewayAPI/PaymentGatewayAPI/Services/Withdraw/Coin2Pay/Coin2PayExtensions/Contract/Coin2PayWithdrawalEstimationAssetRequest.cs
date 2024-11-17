namespace PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract
{
    public class Coin2PayWithdrawalEstimationAssetRequest
    {
        public int platformID { get; set; }
        public double withdrawal_credit { get; set; }
        public string timestamp { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string sign { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Network { get; set; } = string.Empty;
    }
}
