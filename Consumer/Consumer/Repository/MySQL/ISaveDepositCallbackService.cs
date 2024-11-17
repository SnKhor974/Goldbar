using Consumer.Contracts.Callback;

namespace Consumer.Repository.MySQL
{
    public interface ISaveDepositCallbackService
    {
        ValueTask<DepositCallbackResponse> SaveDepositCallback(DepositCallbackResponse response, string merchantName);
    }
}
