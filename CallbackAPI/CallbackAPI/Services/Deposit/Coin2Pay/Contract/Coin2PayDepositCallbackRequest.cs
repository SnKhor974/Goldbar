namespace CallbackAPI.Services.Deposit.Coin2Pay.Contract
{
    public class Coin2PayDepositCallbackRequest
    {
        public string uid { get; set; }
        public string txid { get; set; }
        public string address { get; set; }
        public string token { get; set; }
        public double amount_in_usdt { get; set; }
        public double deposit_fee { get; set; }
        public double quantity { get; set; }
        public double given_Credit { get; set; }
        public double credit_Rate { get; set; }
        public string currency { get; set; }
        public string timestamp { get; set; }
        public string sign { get; set; }

    }
}
