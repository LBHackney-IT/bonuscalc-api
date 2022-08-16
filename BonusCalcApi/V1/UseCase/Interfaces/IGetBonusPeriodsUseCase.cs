using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetBonusPeriodsUseCase
    {
        public Task<IEnumerable<BonusPeriod>> ExecuteAsync();
    }
}
