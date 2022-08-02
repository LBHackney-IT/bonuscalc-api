using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface ICloseBonusPeriodUseCase
    {
        public Task<BonusPeriod> ExecuteAsync(string bonusPeriodId, BonusPeriodUpdate request);
    }
}
