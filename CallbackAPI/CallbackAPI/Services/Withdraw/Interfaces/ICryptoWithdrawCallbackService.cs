using CallbackAPI.Contracts.Enumerables;
using CallbackAPI.Contracts.MerchantSettings;
using Microsoft.AspNetCore.Mvc;

namespace CallbackAPI.Services.Withdraw.Interfaces
{
    public interface ICryptoWithdrawCallbackService
    {
        EnumMerchantContracts Merchants { get; set; }
        ValueTask<IActionResult> ProcessWithdrawCallback(IHttpContextAccessor httpContextAccessor, MerchantSettings merchantSettings);
    }
}
