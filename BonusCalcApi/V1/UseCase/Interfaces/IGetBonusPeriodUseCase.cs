using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetBonusPeriodUseCase
    {
        public Task<BonusPeriod> ExecuteAsync(string id);
    }
}
