using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Npgsql;
using NUnit.Framework;

namespace BonusCalcApi.Tests
{
    [TestFixture]
    public abstract class DatabaseTests
    {
        private IDbContextTransaction _transaction;
        protected BonusCalcContext BonusCalcContext { get; private set; }

        [SetUp]
        public void RunBeforeAnyTests()
        {
            var builder = new DbContextOptionsBuilder<BonusCalcContext>();
            var dataSource = DatabaseContextConfiguration.BuildDataSource(ConnectionString.TestDatabase());

            builder
                .UseNpgsql(dataSource)
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(warnings =>
                        warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));

            BonusCalcContext = new BonusCalcContext(builder.Options);

            BonusCalcContext.Database.Migrate();
            _transaction = BonusCalcContext.Database.BeginTransaction();

            // Empty trades table for tests
            BonusCalcContext.Trades.RemoveRange(BonusCalcContext.Trades);
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
