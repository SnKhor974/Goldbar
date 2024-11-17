using Consumer.Contracts.Enumerables;
using Consumer.Services.Withdraw.Interfaces;
using System.Text.Json;
using Consumer.Services.Withdraw.Coin2Pay.Contract;
using Consumer.Contracts.Callback;
using System.Text;
using Consumer.Repository.MySQL;

namespace Consumer.Services.Withdraw.Coin2Pay
{
    public class Coin2PaySendWithdrawCallbackService : ISendWithdrawCallbackService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISaveWithdrawCallbackService _saveWithdrawCallbackService;
        public EnumMerchantContracts Merchants { get; set; } = EnumMerchantContracts.Coin2Pay;

        public Coin2PaySendWithdrawCallbackService(IHttpClientFactory httpClientFactory, ISaveWithdrawCallbackService saveWithdrawCallbackService)
        {
            _httpClientFactory = httpClientFactory;
            _saveWithdrawCallbackService = saveWithdrawCallbackService;
        }

        public async ValueTask SendWithdraw(string message)
        {
            var jsonMessage = JsonSerializer.Deserialize<Coin2PayWithdrawRequest>(message);

            string[] details = { string.Empty, string.Empty };

            var splitDetails = jsonMessage.order_id.Split(':');
            if (splitDetails.Length == 2)
            {
                details = splitDetails;
            }

            var callbackMessage = new WithdrawCallbackResponse
            {
                UserID = details[0],
                TransactionID = details[1],
                Address = jsonMessage.address,
                Amount = jsonMessage.amount,
                WithdrawAmount = jsonMessage.withdrawal_amount,
                WithdrawFee = jsonMessage.withdrawal_fee
            };

            var endpoint = "https://webhook-test.com/365f13aaeb2b4e4c9d8a191eed152f01";

            var client = _httpClientFactory.CreateClient("WithdrawCallback");
            var jsonCallback = JsonSerializer.Serialize(callbackMessage);
            var callbackContent = new StringContent(jsonCallback, Encoding.UTF8, "application/json");
            var callbackRequest = await client.PostAsync(endpoint, callbackContent);

            await _saveWithdrawCallbackService.SaveWithdrawCallback(callbackMessage, "Coin2Pay");
        }
    }
}
