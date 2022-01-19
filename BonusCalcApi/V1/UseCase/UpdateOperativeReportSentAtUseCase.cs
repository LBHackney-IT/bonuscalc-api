using System;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class UpdateOperativeReportSentAtUseCase : IUpdateOperativeReportSentAtUseCase
    {
        private readonly ITimesheetGateway _timesheetGateway;
        private readonly IDbSaver _dbSaver;

        public UpdateOperativeReportSentAtUseCase(ITimesheetGateway timesheetGateway, IDbSaver dbSaver)
        {
            _timesheetGateway = timesheetGateway;
            _dbSaver = dbSaver;
        }

        public async Task ExecuteAsync(string operativeId, string weekId)
        {
            var timesheet = await _timesheetGateway.GetOperativeTimesheetAsync(operativeId, weekId);

            if (timesheet is null) ThrowHelper.ThrowNotFound($"Timesheet not found for operative: {operativeId} and week: {weekId}");

            if (timesheet.ReportSentAt is null)
            {
                timesheet.ReportSentAt = DateTime.UtcNow;
                await _dbSaver.SaveChangesAsync();
            }
        }
    }
}
