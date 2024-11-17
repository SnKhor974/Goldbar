using PaymentGatewayAPI.Contracts.MerchantSettings;
using PaymentGatewayAPI.Services.Deposit.Coin2Pay;
using PaymentGatewayAPI.Services.Deposit.Interfaces;
using PaymentGatewayAPI.Services.Withdraw.Coin2Pay;
using PaymentGatewayAPI.Services.Withdraw.Interfaces;
using System.Data;
using MySqlConnector;
using PaymentGatewayAPI.Repository.MySQL;
using PaymentGatewayAPI.Contracts.KeySettings;
using PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Interface;
using PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions;

namespace PaymentGatewayAPI
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
            //settings
            services.Configure<List<MerchantSettings>>(Configuration.GetSection("Merchants"));
            services.Configure<List<KeySettings>>(Configuration.GetSection("KeySettings"));        

            //repository
            services.AddTransient<IDbConnection>(s =>
            {
                return new MySqlConnection(Configuration.GetConnectionString("Default"));
            });
            services.AddTransient<ISaveDepositTransactionDetailsService, SaveDepositTransactionDetailsService>();
            services.AddTransient<ISaveWithdrawTransactionDetailsService, SaveWithdrawTransactionDetailsService>();

            //services
            services.AddSingleton<ICreateDepositService, Coin2PayDeposit>();
            services.AddSingleton<ICreateWithdrawService, Coin2PayWithdraw>();
            services.AddSingleton<ICoin2PayWithdrawalEstimationAsset, Coin2PayWithdrawalEstimationAsset>();
            
            services.AddHttpClient("Coin2PayCreateWithdrawOrder", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetValue<string>("Merchants:0:MerchantHost"));
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            services.AddHttpClient("Coin2PayCreateWithdrawOrderAddress", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetValue<string>("Merchants:0:MerchantHost"));
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            services.AddHttpClient("Coin2PayCreateDepositOrder", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetValue<string>("Merchants:0:MerchantHost"));
                client.Timeout = TimeSpan.FromMinutes(5);
            });
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
