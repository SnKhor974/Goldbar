namespace PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Contract
{
    public class Coin2PayWithdrawResponse
    {
        public bool result { get; set; }
        public int code { get; set; }
        public string msg { get; set; } = string.Empty;
        public SubmitData data { get; set; } = new SubmitData();

    }

    public class SubmitData
    {
        public string address { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public double amount { get; set; }
        public double creditamount { get; set; }
        public string txid { get; set; } = string.Empty;
        public string order_id { get; set; } = string.Empty;
    }

}
