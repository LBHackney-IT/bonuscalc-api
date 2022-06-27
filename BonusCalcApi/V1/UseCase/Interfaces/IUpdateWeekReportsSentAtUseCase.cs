using System.Threading.Tasks;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IUpdateWeekReportsSentAtUseCase
    {
        public Task ExecuteAsync(string weekId);
    }
}
