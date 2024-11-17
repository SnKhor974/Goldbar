using PaymentGatewayAPI.Contracts;
using PaymentGatewayAPI.Contracts.Enumerables;
using PaymentGatewayAPI.Contracts.MerchantSettings;

namespace PaymentGatewayAPI.Services.Deposit.Interfaces
{
    public interface ICreateDepositService
    {
        EnumMerchantContracts Merchants { get; set; }
        ValueTask<DepositResponse> CreateDepositOrder(Guid orderID, DepositRequest request, MerchantSettings merchantSettings);
    }
}

