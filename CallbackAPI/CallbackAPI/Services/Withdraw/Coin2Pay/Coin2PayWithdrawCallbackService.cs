using CallbackAPI.Contracts.Enumerables;
using CallbackAPI.Contracts.KeySettings;
using CallbackAPI.Contracts.MerchantSettings;
using CallbackAPI.Queue.Contract;
using CallbackAPI.Queue.Notify;
using CallbackAPI.Services.Deposit.Coin2Pay.Contract;
using CallbackAPI.Services.Withdraw.Coin2Pay.Contract;
using CallbackAPI.Services.Withdraw.Interfaces;
using Extensions.RSA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CallbackAPI.Services.Withdraw.Coin2Pay
{
    public class Coin2PayWithdrawCallbackService: ICryptoWithdrawCallbackService
    {
        public EnumMerchantContracts Merchants { get; set; } = EnumMerchantContracts.Coin2Pay;
        public ILogger<Coin2PayWithdrawCallbackService> _logger { get; set; }

        private readonly INotifyOrderQueue _notifyOrderQueue;
        private readonly IOptions<List<KeySettings>> _keySettings;

        public Coin2PayWithdrawCallbackService(IOptions<List<KeySettings>> keySettings, ILogger<Coin2PayWithdrawCallbackService> logger, INotifyOrderQueue notifyOrderQueue)
        {
            _logger=logger;
            _notifyOrderQueue = notifyOrderQueue;
            _keySettings = keySettings;
        }
        public async ValueTask<IActionResult> ProcessWithdrawCallback(IHttpContextAccessor httpContextAccessor, MerchantSettings merchantSettings)
        {
            try
            {
                //get request content and deserialize
                var content = await httpContextAccessor.HttpContext.Request.ReadFormAsync();
                var jsonContent = content.FirstOrDefault().Key;
                var callbackContent = JsonConvert.DeserializeObject<Coin2PayWithdrawCallbackRequest>(jsonContent);

                //Basestring : uid + txid + address + token + quantity + amount_in_usdt + deposit_fee + given_Credit + credit_Rate + timestamp + currency
                var pureSign = $"{callbackContent.txid}{callbackContent.order_id}{callbackContent.token}{callbackContent.amount}{callbackContent.withdrawal_amount}{callbackContent.withdrawal_fee}{callbackContent.is_green}{callbackContent.timestamp}";

                //validate sign
                /*var key = _keySettings.Value.FirstOrDefault(key => key.Key == "PublicKeyCoin2Pay");
                bool validSign = RSAHelper.VerifySignature(pureSign, callbackContent.sign, key.KeyContent);

                if (!validSign)
                {
                    _logger.LogWarning("Invalid Sign.");
                    return new BadRequestResult();
                }*/

                //add to withdraw queue
                await _notifyOrderQueue.AddToWithdrawQueue(new NotifyOrderRequest
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
