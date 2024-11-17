using Consumer.Contracts.Enumerables;

namespace Consumer.Services.Withdraw.Interfaces
{
    public interface ISendWithdrawCallbackService
    {
        EnumMerchantContracts Merchants { get; }

        ValueTask SendWithdraw(string message);
    }
}
