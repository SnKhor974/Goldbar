using CallbackAPI.Contracts.KeySettings;
using CallbackAPI.Contracts.MerchantSettings;
using CallbackAPI.Services.Deposit.Coin2Pay.Contract;
using CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract;
using CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Interfaces;
using Extensions.RSA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Transactions;

namespace CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions
{
    public class Coin2PayVerifyWithdrawRequestService : ICoin2PayVerifyWithdrawRequestService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<Coin2PayVerifyWithdrawRequestService> _logger;

        public Coin2PayVerifyWithdrawRequestService(ILogger<Coin2PayVerifyWithdrawRequestService> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async ValueTask<Coin2PayVerifyWithdrawResponse> VerifyWithdrawRequest(IHttpContextAccessor httpContextAccessor, MerchantSettings merchantSettings)
        {
            try
            {
                
                var client = _httpClientFactory.CreateClient("Coin2PayVerifyWithdraw");

                var content = await httpContextAccessor.HttpContext.Request.ReadFormAsync();
                var jsonContent = content.FirstOrDefault().Key;
                var callbackContent = JsonConvert.DeserializeObject<Coin2PayVerifyWithdrawRequest>(jsonContent);

               

                //split uid to user id and transaction id
                string[] details = { string.Empty, string.Empty };

                var splitDetails = callbackContent.order_id.Split(':');
                if (splitDetails.Length == 2)
                {
                    details = splitDetails;
                }

                var verifyContent = new GoldbarVerifyWithdrawRequest
                {    
                    UserID = details[0],
                    TransactionID = details[1],
                    Token = callbackContent.token,
                    Network = callbackContent.network,
                    CreditAmount = callbackContent.creditamount,
                    Timestamp = callbackContent.timestamp,
                    Address = callbackContent.address

                 };

                //Goldbar endpoint
                var endpoint = "https://webhook-test.com/a3547a64291900090befa87325239e87";

                var verifyContentHttp = System.Text.Json.JsonSerializer.Serialize(verifyContent);
                var httpContent = new StringContent(verifyContentHttp, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("X-Forwarded-For", merchantSettings.MerchantIP);

                var sendVerify = await client.PostAsync(endpoint, httpContent);

                var merchantResponse = await sendVerify.Content.ReadAsStringAsync();

                var verifyResponse = System.Text.Json.JsonSerializer.Deserialize<Coin2PayVerifyWithdrawResponse>(merchantResponse);

                
                if (verifyResponse.verified == false)
                {
                    return new Coin2PayVerifyWithdrawResponse
                    {
                        verified = false,
                        verifiedmsg = verifyResponse.verifiedmsg
                    };
                }

                return new Coin2PayVerifyWithdrawResponse
                {
                    verified = true,
                    verifiedmsg = verifyResponse.verifiedmsg
                };
              
            }
            catch (Exception ex)
            {
                throw new Exception(nameof(Coin2PayVerifyWithdrawRequestService), ex);
            }
        }

        
    }
}
