namespace PaymentGatewayAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var envName = hostContext.HostingEnvironment.EnvironmentName;
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("configs/appsettings.Merchants.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{envName}.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
                })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
              
            });
    }
}

