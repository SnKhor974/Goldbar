namespace Consumer.Contracts.Callback
{
    /// <summary>
    /// withdraw callback response for frontend
    /// </summary>
    public class WithdrawCallbackResponse
    {
        public string UserID { get; set; } = string.Empty;
        public string TransactionID { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double Amount { get; set; }
        public double WithdrawAmount { get; set; }
        public double WithdrawFee { get; set; }
    }
}
