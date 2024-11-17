namespace PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Contract
{
    public class Coin2PayWithdrawRequest
    {
        public int platformID { get; set; }
        public string from_address { get; set; } = string.Empty;
        public string to_address { get; set; } = string.Empty;
        public string order_id { get; set; } = string.Empty;
        public double creditamount { get; set; }
        public string token { get; set; }
        public string network { get; set; }
        public string timestamp { get; set; } = string.Empty;
        public string sign { get; set; } = string.Empty;
    }
}
