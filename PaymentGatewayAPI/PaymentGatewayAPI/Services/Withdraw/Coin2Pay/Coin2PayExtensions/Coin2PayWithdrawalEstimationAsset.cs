using Extensions.RSA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PaymentGatewayAPI.Contracts.KeySettings;
using PaymentGatewayAPI.Contracts.MerchantSettings;
using PaymentGatewayAPI.Contracts.Order;
using PaymentGatewayAPI.Services.Deposit.Coin2Pay.Contract;
using PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract;
using PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Interface;
using System.Text.Json;
using System.Text;

namespace PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions
{
    public class Coin2PayWithdrawalEstimationAsset : ICoin2PayWithdrawalEstimationAsset
    {
        public ILogger<Coin2PayWithdrawalEstimationAsset> _logger { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<List<KeySettings>> _keySettings;

        public Coin2PayWithdrawalEstimationAsset(ILogger<Coin2PayWithdrawalEstimationAsset> logger, IHttpClientFactory httpClientFactory, IOptions<List<KeySettings>> keySettings)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _keySettings = keySettings;
        }
        public async ValueTask<WithdrawalEstimationAssetResponse> WithdrawalEstimationAsset(WithdrawalEstimationAssetRequest request, MerchantSettings merchantSettings)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Coin2PayWithdrawalEstimationAsset");

                var unixTimestamp = ((DateTimeOffset)DateTime.ParseExact(request.DateTime, "d-M-yyyy HH:mm:ss", null)).ToUnixTimeSeconds().ToString();

                string rawSign = $"171{request.Currency}{unixTimestamp}{request.WithdrawCredit}";

                var key = _keySettings.Value.FirstOrDefault(key => key.Key == "PrivateKeyGoldbar");
                var encryptedSign = RSAHelper.GenerateSign(rawSign, key.KeyContent);

                var orderRequest = new Coin2PayWithdrawalEstimationAssetRequest()
                {
                    //sample
                    platformID = 171, //coin2pay's merchant id
                    withdrawal_credit = request.WithdrawCredit,   
                    timestamp = unixTimestamp,
                    currency = request.Currency,
                    Token = request.Token,
                    Network = request.Network,
                    sign = encryptedSign,
  
                };

                var endpoint = $"{merchantSettings.MerchantHost}/v1/getWithdrawalEstimateValue.ashx";

                var jsonContent = JsonSerializer.Serialize(orderRequest);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("X-Forwarded-For", merchantSettings.MerchantIP);

                var sendRequest = await client.PostAsync(endpoint, httpContent);

                var merchantResponse = await sendRequest.Content.ReadAsStringAsync();

                //parse only asset_Estimate_Get
                using var document = JsonDocument.Parse(merchantResponse);
                var assetArray = document.RootElement
                         .GetProperty("data")
                         .GetProperty("asset_Estimate_get");
                var assetEstimates = JsonSerializer.Deserialize<List<AssetEstimateGet>>(assetArray.ToString());

                //convert to list of dictionary
                var assetList = new List<Dictionary<string, object>>();
                foreach (var asset in assetEstimates)
                {
                    var assetDict = new Dictionary<string, object>
                    {
                        { "Token", asset.Token },
                        { "Estimate_Get_Value", asset.Estimate_Get_Value },
                        
                    };
                    assetList.Add(assetDict);                  
                }

                var coin2PayResponse = JsonSerializer.Deserialize<Coin2PayWithdrawalEstimationAssetResponse>(merchantResponse);

                if (coin2PayResponse.result == false)
                {
                    return new WithdrawalEstimationAssetResponse()
                    {
                        result = false,
                        msg = "Failed to get withdrawal estimation.",
                        WithdrawalCredit = 0,
                        WithdrawalCreditRate = 0,
                        Asset_Estimate_Get = new List<Dictionary<string, object>>()
                    };
                }
                
                return new WithdrawalEstimationAssetResponse()
                {
                    result = true,
                    msg = "Success",
                    WithdrawalCredit = coin2PayResponse.data.withdrawal_Credit,
                    WithdrawalCreditRate = coin2PayResponse.data.withdrawal_credit_rate,
                    Asset_Estimate_Get = assetList
                };
                /*
                if (coin2PayResponse.result == false)
                {
                    return new Coin2PayWithdrawalEstimationAssetResponse()
                    {
                        //failed
                        result = false,
                        code = 400,
                        msg = "Failed to get withdrawal estimation",
                        data.withdrawal_Credit = 0,

                        
                    };
                }

                return new WithdrawResponse()
                {
                    //success
                    OrderID= id,
                    TransactionID = request.TransactionID,
                    WithdrawURL = link,
                    msg = "Success",
                    result = true
                };
                */

            }
            catch (Exception ex)
            {
                throw new Exception(nameof(Coin2PayWithdrawalEstimationAsset), ex);
            }

        }
    }
}
