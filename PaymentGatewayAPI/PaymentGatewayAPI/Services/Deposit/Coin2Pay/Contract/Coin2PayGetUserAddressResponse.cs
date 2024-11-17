namespace PaymentGatewayAPI.Services.Deposit.Coin2Pay.Contract
{
    public class Coin2PayGetUserAddressResponse
    {
        public bool result { get; set; }
        public int code { get; set; }
        public string msg { get; set; } = string.Empty;
        public List<AddressData> data { get; set; } = new List<AddressData>();
        public HostedPage hostedPage { get; set; } = new HostedPage();
    }

    public class AddressData
    {
        public string address { get; set; } = string.Empty;
        public string network { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string label { get; set; } = string.Empty;
    }

    public class HostedPage
    {
        public string HostedPage_Deposit { get; set; } = string.Empty;
        public string HostedPage_Withdrawal { get; set; } = string.Empty;
        public string HostedPage_Withdrawal_B { get; set; } = string.Empty;
    }
}