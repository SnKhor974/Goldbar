using Consumer.Contracts.Enumerables;

namespace Consumer.Services.Deposit.Interfaces
{
    public interface ISendDepositCallbackService
    {
        EnumMerchantContracts Merchants { get; }

        ValueTask SendDeposit(string message);

    }
}
