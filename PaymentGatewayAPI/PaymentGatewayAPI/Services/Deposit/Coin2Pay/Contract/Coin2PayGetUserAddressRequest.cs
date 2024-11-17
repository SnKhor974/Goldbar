namespace PaymentGatewayAPI.Services.Deposit.Coin2Pay.Contract
{
    public class Coin2PayGetUserAddressRequest
    {
        public int platformID { get; set; }
        public string UID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string timestamp { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string sign { get; set; } = string.Empty;
    }
}
