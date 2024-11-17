using CallbackAPI.Queue.Contract;

namespace CallbackAPI.Queue.Notify
{
    public interface INotifyOrderQueue
    {
        ValueTask AddToDepositQueue(NotifyOrderRequest orderRequest);
        ValueTask AddToWithdrawQueue(NotifyOrderRequest orderRequest);
        ValueTask AddToWithdrawFreezeAssetQueue(NotifyOrderRequest orderRequest);
    }
}
