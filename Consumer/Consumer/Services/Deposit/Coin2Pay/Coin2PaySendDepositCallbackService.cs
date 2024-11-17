using Consumer.Contracts.Enumerables;
using Consumer.Services.Deposit.Interfaces;
using Consumer.Services.Deposit.Coin2Pay.Contract;
using Consumer.Contracts.Callback;
using System.Text.Json;
using System.Text;
using Consumer.Repository.MySQL;

namespace Consumer.Services.Deposit.Coin2Pay
{
    public class Coin2PaySendDepositCallbackService: ISendDepositCallbackService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISaveDepositCallbackService _saveDepositCallbackService;

        public EnumMerchantContracts Merchants { get; set; } = EnumMerchantContracts.Coin2Pay;

        public Coin2PaySendDepositCallbackService(IHttpClientFactory httpClientFactory, ISaveDepositCallbackService saveDepositCallbackService)
        {
            _httpClientFactory = httpClientFactory;
            _saveDepositCallbackService = saveDepositCallbackService;
        }

        public async ValueTask SendDeposit(string message)
        {
            var jsonMessage = JsonSerializer.Deserialize<Coin2PayDepositRequest>(message);


            //split uid to user id and transaction id
            string[] details = { string.Empty, string.Empty };

            var splitDetails = jsonMessage.uid.Split(':');
            if (splitDetails.Length == 2)
            {
                details = splitDetails;
            }
            
            //msg for goldbar's endpoint
            var callbackMessage = new DepositCallbackResponse
            {
                UserID = details[0],
                TransactionID = details[1],
                Address = jsonMessage.address,
                DepositAmount = jsonMessage.amount_in_usdt,
                DepositFee = jsonMessage.deposit_fee,
                GivenCredit = jsonMessage.given_Credit

            };

            //goldbar's endpoint
            var endpoint = "https://webhook-test.com/365f13aaeb2b4e4c9d8a191eed152f01";

            var client = _httpClientFactory.CreateClient("DepositCallback");
            var jsonCallback = JsonSerializer.Serialize(callbackMessage);
            var callbackContent = new StringContent(jsonCallback, Encoding.UTF8, "application/json");
            var callbackRequest = await client.PostAsync(endpoint, callbackContent);

            //save to db
            await _saveDepositCallbackService.SaveDepositCallback(callbackMessage, "Coin2Pay");

        }
    }
}
