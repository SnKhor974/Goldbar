using CallbackAPI.Contracts.Enumerables;
using CallbackAPI.Contracts.MerchantSettings;
using CallbackAPI.Queue.Contract;
using CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Interfaces;
using CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract;
using CallbackAPI.Services.Withdraw.Coin2Pay.Contract;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CallbackAPI.Queue.Notify;
using Extensions.RSA;
using CallbackAPI.Contracts.KeySettings;
using Microsoft.Extensions.Options;

namespace CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions
{
    public class Coin2PayWithdrawFreezeAssetCallbackService: ICoin2PayWithdrawFreezeAssetCallbackService
    {
        public ILogger<Coin2PayWithdrawFreezeAssetCallbackService> _logger { get; set; }

        private readonly INotifyOrderQueue _notifyOrderQueue;
        private readonly IOptions<List<KeySettings>> _keySettings;

        public Coin2PayWithdrawFreezeAssetCallbackService(IOptions<List<KeySettings>> keySettings, INotifyOrderQueue notifyOrderQueue, ILogger<Coin2PayWithdrawFreezeAssetCallbackService> logger)
        {
            _notifyOrderQueue = notifyOrderQueue;
            _logger = logger;
            _keySettings = keySettings;
        }
        public async ValueTask<IActionResult> ProcessWithdrawFreezeAssetCallback(IHttpContextAccessor httpContextAccessor, MerchantSettings merchantSettings)
        {
            try
            {
                var content = await httpContextAccessor.HttpContext.Request.ReadFormAsync();
                var jsonContent = content.FirstOrDefault().Key;
                var callbackContent = JsonConvert.DeserializeObject<Coin2PayWithdrawFreezeAssetCallbackRequest>(jsonContent);

                var pureSign = $"{callbackContent.txid}{callbackContent.order_id}{callbackContent.token}{callbackContent.amount}{callbackContent.withdrawal_amount}{callbackContent.withdrawal_fee}{callbackContent.is_green}{callbackContent.timestamp}";

                /*var key = _keySettings.Value.FirstOrDefault(key => key.Key == "PublicKeyCoin2Pay");
                bool validSign = RSAHelper.VerifySignature(pureSign, callbackContent.sign, key.KeyContent);

                if (!validSign)
                {
                    _logger.LogWarning("Invalid Sign.");
                    return new BadRequestResult();
                }*/

                await _notifyOrderQueue.AddToWithdrawFreezeAssetQueue(new NotifyOrderRequest
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
