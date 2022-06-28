using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BonusCalcApi
{
    public static class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSentry(o =>
                    {
                        o.TracesSampleRate = 1.0;
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
