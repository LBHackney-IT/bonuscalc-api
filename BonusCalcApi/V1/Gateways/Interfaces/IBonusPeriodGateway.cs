using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IBonusPeriodGateway
    {
        public Task<BonusPeriod> CreateBonusPeriodAsync(string id);
        public Task<BonusPeriod> GetBonusPeriodAsync(string id);
        public Task<BonusPeriod> GetLastBonusPeriodAsync();
        public Task<IEnumerable<BonusPeriod>> GetBonusPeriodsAsync();
        public Task<IEnumerable<BonusPeriod>> GetCurrentBonusPeriodsAsync(DateTime currentDate);
        public Task<BonusPeriod> GetEarliestOpenBonusPeriodAsync();
    }
}
