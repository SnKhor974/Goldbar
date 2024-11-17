namespace PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract
{
    public class Coin2PayWithdrawalEstimationAssetResponse
    {
        public bool result { get; set; }
        public int code { get; set; }
        public string msg { get; set; } = string.Empty;
        public EstimateData data { get; set; } = new EstimateData();
    }

    public class EstimateData
    {
        public double withdrawal_Credit { get; set; }
        public double withdrawal_credit_rate { get; set; }
        public List<AssetEstimateGet> asset_Estimate_Get { get; set; } = new List<AssetEstimateGet>();
    }

    public class AssetEstimateGet
    {
        public string Token { get; set; } = string.Empty;
        public double Estimate_Get_Value { get; set; }
        public double Estimate_Get_Market_Value { get; set; }
        public double Token_Price { get; set; }
        public double Token_Price_Market { get; set; }
    }
}
