using Consumer.Contracts.Callback;

namespace Consumer.Repository.MySQL
{
    public interface ISaveWithdrawCallbackService
    {
        ValueTask<WithdrawCallbackResponse> SaveWithdrawCallback(WithdrawCallbackResponse response, string merchantName);
    }
}
