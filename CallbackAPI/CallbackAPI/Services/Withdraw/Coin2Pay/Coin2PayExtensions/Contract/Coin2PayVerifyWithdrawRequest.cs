namespace CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract
{
    /// <summary>
    /// verify withdraw request from coin2pay
    /// </summary>
    public class Coin2PayVerifyWithdrawRequest
    {
        public string order_id { get; set; } = string.Empty;
        public string UID { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string network { get; set; } = string.Empty;
        public double creditamount { get; set; }
        public string timestamp { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string sign { get; set; } = string.Empty;
    }
}
