using CallbackAPI.Contracts.MerchantSettings;
using CallbackAPI.Services.Deposit.Interfaces;
using CallbackAPI.Services.Withdraw.Interfaces;
using CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract;

namespace CallbackAPI.Controllers
{
    [ApiController]
    public class CryptoController : ControllerBase
    {
        private readonly IEnumerable<ICryptoDepositCallbackService> _cryptoDepositCallback;
        private readonly IEnumerable<ICryptoWithdrawCallbackService> _cryptoWithdrawCallback;
  
        
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly IOptions<List<MerchantSettings>> _options;

        private readonly ICoin2PayWithdrawFreezeAssetCallbackService _coin2PayWithdrawFreezeAssetCallback;
        private readonly ICoin2PayVerifyWithdrawRequestService _coin2PayVerifyWithdrawRequest;

        

        public CryptoController(IHttpContextAccessor httpcontextAccessor, IOptions<List<MerchantSettings>> options, IEnumerable<ICryptoDepositCallbackService> cryptoDepositCallback, IEnumerable<ICryptoWithdrawCallbackService> cryptoWithdrawCallback, ICoin2PayWithdrawFreezeAssetCallbackService coin2PayWithdrawFreezeAssetCallback, ICoin2PayVerifyWithdrawRequestService coin2PayVerifyWithdrawRequest)
        {
            _httpcontextAccessor = httpcontextAccessor;
            _options = options;
            _cryptoDepositCallback = cryptoDepositCallback;
            _cryptoWithdrawCallback = cryptoWithdrawCallback;
            _coin2PayWithdrawFreezeAssetCallback = coin2PayWithdrawFreezeAssetCallback;
            _coin2PayVerifyWithdrawRequest = coin2PayVerifyWithdrawRequest;
        }

        [HttpPost("/Deposit/{merchantName}")]
        public async ValueTask<IActionResult>DepositCallback(string merchantName)
        {
            var merchantSettings = _options.Value.FirstOrDefault(name => name.MerchantName == merchantName);
            if (merchantSettings == null) return NotFound();

            var selectMerchant = _cryptoDepositCallback.FirstOrDefault(merchant => merchant.Merchants.ToString() == merchantSettings.MerchantName);
            if (selectMerchant == null) return NotFound();
            
            var result = await selectMerchant.ProcessDepositCallback(_httpcontextAccessor, merchantSettings);

            return result;
        }

        [HttpPost("/Withdraw/{merchantName}")]
        public async ValueTask<IActionResult>WithdrawCallback(string merchantName)
        {
            var merchantSettings = _options.Value.FirstOrDefault(name => name.MerchantName == merchantName);
            if (merchantSettings == null) return NotFound();

            var selectMerchant = _cryptoWithdrawCallback.FirstOrDefault(merchant => merchant.Merchants.ToString() == merchantSettings.MerchantName);
            if (selectMerchant == null) return NotFound();

            var result = await selectMerchant.ProcessWithdrawCallback(_httpcontextAccessor, merchantSettings);

            return result;
        }


        //Coin2Pay extensions
        /*[HttpPost("/WithdrawFreezeAsset/Coin2Pay")]
        public async ValueTask<IActionResult> WithdrawFreezeAssetCallback()
        {
            Console.WriteLine("WithdrawFreezeAssetCallback");
            var merchantSettings = _options.Value.FirstOrDefault(name => name.MerchantName == "Coin2Pay");
            if (merchantSettings == null) return NotFound();

            var result = await _coin2PayWithdrawFreezeAssetCallback.ProcessWithdrawFreezeAssetCallback(_httpcontextAccessor, merchantSettings);

            return result;
        }*/

        [HttpPost("/WithdrawVerify/Coin2Pay")]
        public async ValueTask<IActionResult> VerifyWithdraw()
        {
            var merchantSettings = _options.Value.FirstOrDefault(name => name.MerchantName == "Coin2Pay");
            if (merchantSettings == null) return NotFound();
   
            var result = await _coin2PayVerifyWithdrawRequest.VerifyWithdrawRequest(_httpcontextAccessor, merchantSettings);

            
            
            return new JsonResult(result);
        }



    }
}
