using PaymentGatewayAPI.Contracts.Order;
using PaymentGatewayAPI.Contracts.Enumerables;
using PaymentGatewayAPI.Contracts.MerchantSettings;

namespace PaymentGatewayAPI.Services.Withdraw.Interfaces
{
    public interface ICreateWithdrawService
    {
        EnumMerchantContracts Merchants { get; set; }
        ValueTask<WithdrawResponse> CreateWithdrawOrder(Guid id, WithdrawRequest request, MerchantSettings merchantSettings);
    }
}
