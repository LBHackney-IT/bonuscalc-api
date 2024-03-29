using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface ITimesheetGateway
    {
        public Task<Timesheet> GetOperativeTimesheetAsync(string operativeId, string weekId);
    }
}
