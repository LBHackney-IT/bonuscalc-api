using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetOperativeSummaryUseCase
    {
        public Task<Summary> ExecuteAsync(string operativeId, string bonusPeriodId);
    }
}
