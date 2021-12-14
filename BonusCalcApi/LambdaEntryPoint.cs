using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;

namespace BonusCalcApi
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseSentry(o =>
            {
                o.TracesSampleRate = 1.0;
                o.FlushOnCompletedRequest = true;
            });
            builder.UseStartup<Startup>();
        }
    }
}
