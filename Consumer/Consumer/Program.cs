using Consumer.Consumer;
using MySqlConnector;
using System.Data;
using Consumer.Services.Deposit.Interfaces;
using Consumer.Services.Deposit.Coin2Pay;
using Consumer.Services.Withdraw.Interfaces;
using Consumer.Repository.MySQL;
using Consumer.Services.Withdraw.Coin2Pay;
using Extensions.Polly;
using Extensions.EasyNetQ;


namespace Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(configHost =>
            {
                configHost.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("configs/appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
            })
            .ConfigureServices((hostContext, services) =>
            {
                //repository
                services.AddTransient<IDbConnection>(s =>
                {
                    return new MySqlConnection(hostContext.Configuration.GetConnectionString("Default"));
                });

                services.AddTransient<ISaveDepositCallbackService, SaveDepositCallbackService>();
                services.AddTransient<ISaveWithdrawCallbackService, SaveWithdrawCallbackService>();

                //application (backgroundservice)
                services.AddHostedService<CryptoDepositConsumer>();
                services.AddHostedService<CryptoWithdrawConsumer>();

                services.UseEasyNetQ(hostContext.Configuration.GetValue<string>("queues:rabbitmq"));

                services.AddHttpClient("DepositCallback", c =>
                {
                    c.Timeout = TimeSpan.FromMinutes(5);
                }).AddPolicyHandler(PollyRetryPolicy.Policy);

                services.AddHttpClient("WithdrawCallback", c =>
                {
                    c.Timeout = TimeSpan.FromMinutes(5);
                }).AddPolicyHandler(PollyRetryPolicy.Policy);

                //services
                services.AddSingleton<ISendDepositCallbackService, Coin2PaySendDepositCallbackService>();
                services.AddSingleton<ISendWithdrawCallbackService, Coin2PaySendWithdrawCallbackService>();
        
            });
        
    }
}