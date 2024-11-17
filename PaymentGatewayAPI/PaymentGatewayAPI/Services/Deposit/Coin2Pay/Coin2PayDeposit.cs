using PaymentGatewayAPI.Contracts;
using PaymentGatewayAPI.Contracts.Enumerables;
using PaymentGatewayAPI.Contracts.MerchantSettings;
using PaymentGatewayAPI.Services.Deposit.Coin2Pay.Contract;
using PaymentGatewayAPI.Services.Deposit.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Extensions.RSA;
using PaymentGatewayAPI.Contracts.KeySettings;
using Microsoft.Extensions.Options;
using Google.Protobuf.WellKnownTypes;

namespace PaymentGatewayAPI.Services.Deposit.Coin2Pay
{
    public class Coin2PayDeposit : ICreateDepositService
    {
        public EnumMerchantContracts Merchants { get; set; } = EnumMerchantContracts.Coin2Pay;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<List<KeySettings>> _keySettings;

        public Coin2PayDeposit(IHttpClientFactory httpClientFactory, IOptions<List<KeySettings>> keySettings)
        {
            _httpClientFactory = httpClientFactory;
            _keySettings = keySettings;
        }

        public async ValueTask<DepositResponse> CreateDepositOrder(Guid id, DepositRequest request, MerchantSettings merchantSettings)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Coin2PayCreateDepositOrder");

                var unixTimestamp = ((DateTimeOffset)DateTime.ParseExact(request.DateTime, "d-M-yyyy HH:mm:ss", null)).ToUnixTimeSeconds().ToString();

                var user_orderID = $"{request.UserID}:{request.TransactionID}";

                string rawSign = $"{merchantSettings.MerchantAccountID}{user_orderID}{merchantSettings.MerchantAccountName}{unixTimestamp}";

                var key = _keySettings.Value.FirstOrDefault(key => key.Key == "PrivateKeyGoldbar");
                var encryptedSign = RSAHelper.GenerateSign(rawSign, key.KeyContent);

                var orderRequest = new Coin2PayGetUserAddressRequest()
                {
                    //sample
                    platformID = merchantSettings.MerchantAccountID, //coin2pay's merchant id
                    UID = user_orderID, //goldbar's member ID + orderID to identify the order
                    UserName = merchantSettings.MerchantAccountName, //coin2pay's merchant name
                    timestamp = unixTimestamp,
                    currency = request.Currency,
                    sign = encryptedSign

                };

                var endpoint = $"{merchantSettings.MerchantHost}/v1/GetAddress.ashx";

                var jsonContent = JsonSerializer.Serialize(orderRequest);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("X-Forwarded-For", merchantSettings.MerchantIP);

                var sendRequest = await client.PostAsync(endpoint, httpContent);
                
                var merchantResponse = await sendRequest.Content.ReadAsStringAsync();

                var coin2PayResponse = JsonSerializer.Deserialize<Coin2PayGetUserAddressResponse>(merchantResponse);

                if (coin2PayResponse.result == false)
                {
                    return new DepositResponse()
                    {
                        //failed
                        OrderID = id,
                        TransactionID = request.TransactionID,
                        DepositURL = "",
                        msg = $"Failed: {coin2PayResponse.msg}",
                        result = false
                    };
                }

                return new DepositResponse()
                {
                    //success
                    OrderID = id,
                    TransactionID = request.TransactionID,
                    DepositURL = coin2PayResponse.hostedPage.HostedPage_Deposit,
                    msg = "Success",
                    result = true
                };

            }
            catch (Exception ex) {
                throw new Exception(nameof(Coin2PayDeposit), ex);
            }
        }
    }
    
}
