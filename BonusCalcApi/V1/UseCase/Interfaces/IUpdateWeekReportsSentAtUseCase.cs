using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IUpdateWeekReportsSentAtUseCase
    {
        public Task ExecuteAsync(string weekId);
    }
}
