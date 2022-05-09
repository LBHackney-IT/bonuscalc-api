using System;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class UpdateWeekReportsSentAtUseCase : IUpdateWeekReportsSentAtUseCase
    {
        private readonly IWeekGateway _weekGateway;
        private readonly IDbSaver _dbSaver;

        public UpdateWeekReportsSentAtUseCase(IWeekGateway weekGateway, IDbSaver dbSaver)
        {
            _weekGateway = weekGateway;
            _dbSaver = dbSaver;
        }

        public async Task ExecuteAsync(string weekId)
        {
            var week = await _weekGateway.GetWeekAsync(weekId);

            if (week is null)
            {
                ThrowHelper.ThrowNotFound($"Week not found for: {weekId}");
            }
            else if (week.ReportsSentAt is null)
            {
                week.ReportsSentAt = DateTime.UtcNow;
                await _dbSaver.SaveChangesAsync();
            }
        }
    }
}
