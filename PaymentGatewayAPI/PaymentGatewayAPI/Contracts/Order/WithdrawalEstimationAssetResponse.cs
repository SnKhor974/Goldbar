namespace PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract
{
    public class WithdrawalEstimationAssetResponse
    {
        public bool result { get; set; }
        public string msg { get; set; } = string.Empty;
        public double WithdrawalCredit { get; set; }
        public double WithdrawalCreditRate { get; set; }
        public List<Dictionary<string, object>> Asset_Estimate_Get { get; set; }
    }
}