using CallbackAPI.Contracts.MerchantSettings;
using CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract;

namespace CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Interfaces
{
    public interface ICoin2PayVerifyWithdrawRequestService
    {
        ValueTask<Coin2PayVerifyWithdrawResponse> VerifyWithdrawRequest(IHttpContextAccessor httpContextAccessor, MerchantSettings merchantSettings);
    }
}
