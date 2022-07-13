using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetBonusPeriodForChangesUseCase
    {
        public Task<BonusPeriod> ExecuteAsync();
    }
}
