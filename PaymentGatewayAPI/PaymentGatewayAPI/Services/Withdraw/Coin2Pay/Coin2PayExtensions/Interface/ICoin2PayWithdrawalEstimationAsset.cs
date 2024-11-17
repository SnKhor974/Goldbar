using Microsoft.AspNetCore.Mvc;
using PaymentGatewayAPI.Contracts.MerchantSettings;
using PaymentGatewayAPI.Contracts.Order;
using PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract;

namespace PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Interface
{
    public interface ICoin2PayWithdrawalEstimationAsset
    {
        ValueTask<WithdrawalEstimationAssetResponse> WithdrawalEstimationAsset(WithdrawalEstimationAssetRequest request, MerchantSettings merchantSettings);
    }
}
