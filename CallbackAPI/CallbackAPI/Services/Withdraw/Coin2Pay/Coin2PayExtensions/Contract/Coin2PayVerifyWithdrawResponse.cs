namespace CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract
{
    /// <summary>
    /// verify withdraw response to coin2pay
    /// </summary>
    public class Coin2PayVerifyWithdrawResponse
    {
        public string verifiedmsg { get; set; } = string.Empty;
        public bool verified { get; set; }
    }
}
