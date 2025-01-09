using BonusCalcApi.V1.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BonusCalcApi.Tests
{
    public class MockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly DbContextOptionsBuilder _builder;

        public MockWebApplicationFactory(DbContextOptionsBuilder builder)
        {
            _builder = builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                Context = new BonusCalcContext(_builder.Options);
                services.AddSingleton(Context);

                var serviceProvider = services.BuildServiceProvider();
                var dbContext = serviceProvider.GetRequiredService<BonusCalcContext>();

                dbContext.Database.Migrate();
            });
        }
        public BonusCalcContext Context { get; set; }
    }
}
