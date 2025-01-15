using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace BonusCalcApi.V1.Gateways
{
    public static class DatabaseContextConfiguration
    {
        public static DbContextOptionsBuilder ConfigureContext(
            this DbContextOptionsBuilder options,
            string connectionString)
        {
            return ConfigureTheContext(options, connectionString);
        }

        public static DbContextOptionsBuilder<BonusCalcContext> ConfigureContext(
            this DbContextOptionsBuilder<BonusCalcContext> options,
            string connectionString)
        {
            return (DbContextOptionsBuilder<BonusCalcContext>) ConfigureTheContext(options, connectionString);
        }

        private static DbContextOptionsBuilder ConfigureTheContext(
            DbContextOptionsBuilder options,
            string connectionString)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.MapEnum<BandChangeDecision>("band_change_decision");
            var dataSource = dataSourceBuilder.Build();

            return options
                .UseNpgsql(dataSource)
                .UseSnakeCaseNamingConvention();
        }

        public static DbContextOptionsBuilder<BonusCalcContext> IgnoreManyServiceProvidersWarning(
             this DbContextOptionsBuilder<BonusCalcContext> options)
        {
            return options.ConfigureWarnings(warnings =>
                warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
        }
    }
}
