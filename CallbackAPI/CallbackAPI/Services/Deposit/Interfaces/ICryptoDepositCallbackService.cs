using CallbackAPI.Contracts.Enumerables;
using CallbackAPI.Contracts.MerchantSettings;
using Microsoft.AspNetCore.Mvc;

namespace CallbackAPI.Services.Deposit.Interfaces
{
    public interface ICryptoDepositCallbackService
    {
        EnumMerchantContracts Merchants { get; set; }
        ValueTask<IActionResult> ProcessDepositCallback(IHttpContextAccessor httpContextAccessor, MerchantSettings merchantSettings);
    }
}
