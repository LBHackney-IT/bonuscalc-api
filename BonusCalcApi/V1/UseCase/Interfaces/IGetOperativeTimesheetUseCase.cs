using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetOperativeTimesheetUseCase
    {
        public Task<Timesheet> ExecuteAsync(string operativeId, string weekId);
    }

}
