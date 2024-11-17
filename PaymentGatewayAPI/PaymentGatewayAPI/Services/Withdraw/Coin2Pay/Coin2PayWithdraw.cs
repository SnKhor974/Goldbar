using PaymentGatewayAPI.Contracts;
using PaymentGatewayAPI.Contracts.Enumerables;
using PaymentGatewayAPI.Contracts.MerchantSettings;
using PaymentGatewayAPI.Contracts.Order;
using PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Contract;
using PaymentGatewayAPI.Services.Withdraw.Interfaces;
using System.Text.Json;
using System.Text;
using PaymentGatewayAPI.Services.Deposit.Coin2Pay.Contract;
using Extensions.RSA;
using Microsoft.Extensions.Options;
using PaymentGatewayAPI.Contracts.KeySettings;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PaymentGatewayAPI.Services.Withdraw.Coin2Pay
{
    public class Coin2PayWithdraw : ICreateWithdrawService
    {
        public EnumMerchantContracts Merchants { get; set; } = EnumMerchantContracts.Coin2Pay;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<List<KeySettings>> _keySettings;

        public Coin2PayWithdraw(IHttpClientFactory httpClientFactory, IOptions<List<KeySettings>> keySettings)
        {
            _httpClientFactory = httpClientFactory;
            _keySettings = keySettings;
        }

        public async ValueTask<WithdrawResponse> CreateWithdrawOrder(Guid id, WithdrawRequest request, MerchantSettings merchantSettings)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Coin2PayCreateWithdrawOrder");
                var clientAddress = _httpClientFactory.CreateClient("Coin2PayCreateWithdrawOrderAddress");

                var unixTimestamp = ((DateTimeOffset)DateTime.ParseExact(request.DateTime, "d-M-yyyy HH:mm:ss", null)).ToUnixTimeSeconds().ToString();

                var user_orderID = $"{request.UserID}:{request.TransactionID}";              

                string rawSignAddress = $"{merchantSettings.MerchantAccountID}{user_orderID}{merchantSettings.MerchantAccountName}{unixTimestamp}";

                var key = _keySettings.Value.FirstOrDefault(key => key.Key == "PrivateKeyGoldbar");
                var encryptedSignAddress = RSAHelper.GenerateSign(rawSignAddress, key.KeyContent);

                var orderRequestAddress = new Coin2PayGetUserAddressRequest()
                {
                    //sample
                    platformID = merchantSettings.MerchantAccountID, //coin2pay's merchant id
                    UID = user_orderID, //goldbar's member ID + orderID to identify the order
                    UserName = merchantSettings.MerchantAccountName, //coin2pay's merchant name
                    timestamp = unixTimestamp,
                    currency = "",
                    sign = encryptedSignAddress

                };

                var endpointAddress = $"{merchantSettings.MerchantHost}/v1/GetAddress.ashx";

                var jsonContentAddress = JsonSerializer.Serialize(orderRequestAddress);
                var httpContentAddress = new StringContent(jsonContentAddress, Encoding.UTF8, "application/json");
                clientAddress.DefaultRequestHeaders.Add("X-Forwarded-For", merchantSettings.MerchantIP);

                var sendRequestAddress = await clientAddress.PostAsync(endpointAddress, httpContentAddress);

                var merchantResponseAddress = await sendRequestAddress.Content.ReadAsStringAsync();

                var coin2PayResponseAddress = JsonSerializer.Deserialize<Coin2PayGetUserAddressResponse>(merchantResponseAddress);

                //address here
                var walletDetails = coin2PayResponseAddress.data.FirstOrDefault(wallet => wallet.token == request.Token && wallet.network == request.Network);
                if (walletDetails == null)
                {
                    return new WithdrawResponse()
                    {
                        //failed
                        OrderID= id,
                        TransactionID = request.TransactionID,
                        WithdrawURL = "",
                        msg = "Failed: Token or network not supported",
                        result = false
                    };
                }

                var from_address = walletDetails.address;
                var to_address = request.WalletAddress;             

                string rawSign = $"{merchantSettings.MerchantAccountID}{from_address}{to_address}{user_orderID}{request.TotalAmount}{walletDetails.token}{walletDetails.network}{unixTimestamp}";
                var encryptedSign = RSAHelper.GenerateSign(rawSign, key.KeyContent);

                var orderRequest = new Coin2PayWithdrawRequest()
                {
                    platformID = merchantSettings.MerchantAccountID, //coin2pay's merchant id
                    from_address = from_address,
                    to_address = to_address,
                    order_id = user_orderID,
                    creditamount = request.TotalAmount,
                    token = walletDetails.token,
                    network = walletDetails.network,
                    timestamp = unixTimestamp,
                    sign = encryptedSign

                };

                var endpoint = $"{merchantSettings.MerchantHost}/v1/RequestWithdraw.ashx";

                var jsonContent = JsonSerializer.Serialize(orderRequest);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("X-Forwarded-For", merchantSettings.MerchantIP);

                var sendRequest = await client.PostAsync(endpoint, httpContent);

                var merchantResponse = await sendRequest.Content.ReadAsStringAsync();              

                var coin2PayResponse = JsonSerializer.Deserialize<Coin2PayWithdrawResponse>(merchantResponse);
                if (coin2PayResponse.result == false)
                {
                    return new WithdrawResponse()
                    {
                        //failed
                        OrderID= id,
                        TransactionID = request.TransactionID,
                        WithdrawURL = "",
                        msg = $"Failed: {coin2PayResponse.msg}",
                        result = false
                    };
                }

                return new WithdrawResponse()
                {
                    //success
                    OrderID= id,
                    TransactionID = request.TransactionID,
                    WithdrawURL = "",
                    msg = "Success",
                    result = true
                };
                

            }
            catch (Exception ex)
            {
                throw new Exception(nameof(Coin2PayWithdraw), ex);
            }
        }
    }
}
