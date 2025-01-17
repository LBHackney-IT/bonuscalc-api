using BonusCalcApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace BonusCalcApi.V1.Gateways
{
    public static class DatabaseContextConfiguration
    {
        public static NpgsqlDataSource BuildDataSource(string connectionString)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

            dataSourceBuilder.MapEnum<BandChangeDecision>("band_change_decision");

            return dataSourceBuilder.Build();
        }
    }
}
