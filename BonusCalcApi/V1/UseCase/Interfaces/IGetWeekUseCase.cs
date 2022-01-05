using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetWeekUseCase
    {
        public Task<Week> ExecuteAsync(string weekId);
    }
}
