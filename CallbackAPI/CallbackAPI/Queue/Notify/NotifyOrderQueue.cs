using CallbackAPI.Queue.Contract;
using EasyNetQ;
using System.Text.Json;
using System.Threading.Tasks;

namespace CallbackAPI.Queue.Notify
{
    public class NotifyOrderQueue : INotifyOrderQueue
    {
        private readonly IBus _bus;

        public NotifyOrderQueue(IBus bus)
        {
            _bus = bus;
        }
        public async ValueTask AddToDepositQueue(NotifyOrderRequest orderRequest)
        {
            var requestString = System.Text.Json.JsonSerializer.Serialize(orderRequest);
            await _bus.SendReceive.SendAsync("DepositQueue", requestString);

        }
        public async ValueTask AddToWithdrawQueue(NotifyOrderRequest orderRequest)
        {
            var requestString = System.Text.Json.JsonSerializer.Serialize(orderRequest);
            await _bus.SendReceive.SendAsync("WithdrawQueue", requestString);
        }

        public async ValueTask AddToWithdrawFreezeAssetQueue(NotifyOrderRequest orderRequest)
        {
            var requestString = System.Text.Json.JsonSerializer.Serialize(orderRequest);
            await _bus.SendReceive.SendAsync("WithdrawFreezeAssetQueue", requestString);
        }
    }
}
