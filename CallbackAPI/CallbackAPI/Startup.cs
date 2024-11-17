using CallbackAPI.Contracts.MerchantSettings;
using CallbackAPI.Services.Deposit.Coin2Pay;
using CallbackAPI.Services.Deposit.Interfaces;
using CallbackAPI.Services.Withdraw.Coin2Pay;
using CallbackAPI.Services.Withdraw.Interfaces;
using CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions;
using CallbackAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Interfaces;
using Extensions.EasyNetQ;
using CallbackAPI.Queue.Notify;
using CallbackAPI.Contracts.KeySettings;
namespace CallbackAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddHttpClient("Coin2PayVerifyWithdraw", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetValue<string>("Merchants:0:MerchantHost"));
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            //queue
            services.UseEasyNetQ(Configuration.GetValue<string>("queues:rabbitmq"));

            //settings
            services.Configure<List<MerchantSettings>>(Configuration.GetSection("Merchants"));
            services.Configure<List<KeySettings>>(Configuration.GetSection("KeySettings"));

            //services
            services.AddSingleton<ICryptoDepositCallbackService, Coin2PayDepositCallbackService>();
            services.AddSingleton<ICryptoWithdrawCallbackService, Coin2PayWithdrawCallbackService>();
            services.AddSingleton<ICoin2PayWithdrawFreezeAssetCallbackService, Coin2PayWithdrawFreezeAssetCallbackService>();
            services.AddSingleton<ICoin2PayVerifyWithdrawRequestService, Coin2PayVerifyWithdrawRequestService>();
            services.AddSingleton<INotifyOrderQueue, NotifyOrderQueue>();

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
