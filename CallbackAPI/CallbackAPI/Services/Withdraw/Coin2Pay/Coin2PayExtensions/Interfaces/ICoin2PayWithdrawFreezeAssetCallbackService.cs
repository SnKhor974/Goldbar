using CallbackAPI.Contracts.Enumerables;
using CallbackAPI.Contracts.MerchantSettings;
using Microsoft.AspNetCore.Mvc;

namespace CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Interfaces
{
    public interface ICoin2PayWithdrawFreezeAssetCallbackService
    {
        ValueTask<IActionResult> ProcessWithdrawFreezeAssetCallback(IHttpContextAccessor httpContextAccessor, MerchantSettings merchantSettings);

    }
}
