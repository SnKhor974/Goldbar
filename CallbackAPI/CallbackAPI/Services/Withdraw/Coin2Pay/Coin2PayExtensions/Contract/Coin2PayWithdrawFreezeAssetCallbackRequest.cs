namespace CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract
{
    /// <summary>
    /// freeze asset callback from coin2pay
    /// </summary>
    public class Coin2PayWithdrawFreezeAssetCallbackRequest
    {
        public string uid { get; set; }
        public string txid { get; set; }
        public string address { get; set; }
        public string order_id { get; set; }
        public string token { get; set; }
        public double amount { get; set; }
        public int is_green { get; set; }
        public double withdrawal_amount { get; set; }
        public double withdrawal_fee { get; set; }
        public string timestamp { get; set; }
        public string sign { get; set; }

    }
}
