namespace PaymentGatewayAPI.Contracts.Order
{
    public class WithdrawalEstimationAssetRequest
    {
        public double WithdrawCredit { get; set; }
        public string DateTime { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Network { get; set; } = string.Empty;
    }
}
