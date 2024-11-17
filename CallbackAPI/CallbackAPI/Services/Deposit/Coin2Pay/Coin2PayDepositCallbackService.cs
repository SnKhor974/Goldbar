using CallbackAPI.Contracts.Enumerables;
using CallbackAPI.Contracts.MerchantSettings;
using CallbackAPI.Services.Deposit.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CallbackAPI.Services.Deposit.Coin2Pay.Contract;
using CallbackAPI.Queue.Contract;
using CallbackAPI.Queue.Notify;
using Extensions.RSA;
using CallbackAPI.Contracts.KeySettings;
using Microsoft.Extensions.Options;

namespace CallbackAPI.Services.Deposit.Coin2Pay
{
    public class Coin2PayDepositCallbackService : ICryptoDepositCallbackService
    {
        public EnumMerchantContracts Merchants { get; set; } = EnumMerchantContracts.Coin2Pay;
        public ILogger<Coin2PayDepositCallbackService> _logger { get; set; }

        private readonly INotifyOrderQueue _notifyOrderQueue;
        private readonly IOptions<List<KeySettings>> _keySettings;

        public Coin2PayDepositCallbackService(IOptions<List<KeySettings>> keySettings, ILogger<Coin2PayDepositCallbackService> logger, INotifyOrderQueue notifyOrderQueue)
        {
            _logger=logger;
            _notifyOrderQueue = notifyOrderQueue;
            _keySettings = keySettings;

        }

        public async ValueTask<IActionResult> ProcessDepositCallback(IHttpContextAccessor httpContextAccessor, MerchantSettings merchantSettings)
        {
            try
            {
                var content = await httpContextAccessor.HttpContext.Request.ReadFormAsync();
                var jsonContent = content.FirstOrDefault().Key;
                var callbackContent = JsonConvert.DeserializeObject<Coin2PayDepositCallbackRequest>(jsonContent);

                var pureSign = $"{callbackContent.uid}{callbackContent.txid}{callbackContent.address}{callbackContent.token}{callbackContent.quantity}{callbackContent.amount_in_usdt}{callbackContent.deposit_fee}{callbackContent.given_Credit}{callbackContent.credit_Rate}{callbackContent.timestamp}{callbackContent.currency}";
            
                /*

                var key = _keySettings.Value.FirstOrDefault(key => key.Key == "PublicKeyCoin2Pay");
                bool validSign = RSAHelper.VerifySignature(pureSign, callbackContent.sign, key.KeyContent);
                
                if (!validSign)
                {
                    _logger.LogWarning("Invalid Sign.");
                    return new BadRequestResult();
                }*/

                await _notifyOrderQueue.AddToDepositQueue(new NotifyOrderRequest
                {
                    Merchants = EnumMerchantContracts.Coin2Pay,
                    orderRequest = jsonContent
                });

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new BadRequestResult();
            }
        }
    }
}
