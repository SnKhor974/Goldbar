using Consumer.Contracts.Deposit;
using Consumer.Services.Deposit.Interfaces;
using EasyNetQ;
using System.Text.Json;

namespace Consumer.Consumer
{
    public sealed class CryptoDepositConsumer : BackgroundService
    {
        private readonly IBus _bus;
        private readonly IEnumerable<ISendDepositCallbackService> _depositCallbackServices;
        

        public CryptoDepositConsumer(IBus bus, IEnumerable<ISendDepositCallbackService> depositCallbackService)
        {
            _bus = bus;
            _depositCallbackServices = depositCallbackService;
        }
     
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bus.SendReceive.ReceiveAsync<string>("DepositQueue", async message =>
            {
                var cryptoDepositMessage = System.Text.Json.JsonSerializer.Deserialize<NotifyOrderRequest>(message);
                
                var selectMerchant = _depositCallbackServices.FirstOrDefault(x => x.Merchants == cryptoDepositMessage.Merchants);

                await selectMerchant.SendDeposit(cryptoDepositMessage.orderRequest);

            }, cancellationToken: stoppingToken);
        }
    }
}
