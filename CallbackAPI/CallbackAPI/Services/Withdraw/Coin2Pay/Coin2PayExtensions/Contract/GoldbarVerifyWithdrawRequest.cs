namespace CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract
{
    public class GoldbarVerifyWithdrawRequest
    {
        public string TransactionID { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Network { get; set; } = string.Empty;
        public double CreditAmount { get; set; }
        public string Timestamp { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
