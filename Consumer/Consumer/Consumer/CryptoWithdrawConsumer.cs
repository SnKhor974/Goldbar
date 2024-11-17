using Consumer.Services.Withdraw.Interfaces;
using EasyNetQ;
using Consumer.Contracts.Deposit;
using System.Text.Json;

namespace Consumer.Consumer
{
    public class CryptoWithdrawConsumer: BackgroundService
    {
        private readonly IBus _bus;
        private readonly IEnumerable<ISendWithdrawCallbackService> _withdrawCallbackServices;


        public CryptoWithdrawConsumer(IBus bus, IEnumerable<ISendWithdrawCallbackService> depositCallbackService)
        {
            _bus = bus;
            _withdrawCallbackServices = depositCallbackService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bus.SendReceive.ReceiveAsync<string>("WithdrawQueue", async message =>
            {
                var cryptoDepositMessage = System.Text.Json.JsonSerializer.Deserialize<NotifyOrderRequest>(message);

                var selectMerchant = _withdrawCallbackServices.FirstOrDefault(x => x.Merchants == cryptoDepositMessage.Merchants);

                await selectMerchant.SendWithdraw(cryptoDepositMessage.orderRequest);

            }, cancellationToken: stoppingToken);
        }
    }
}
