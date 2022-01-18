using System;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class UpdateReportsSentAtUseCase : IUpdateReportsSentAtUseCase
    {
        private readonly IWeekGateway _weekGateway;
        private readonly IDbSaver _dbSaver;

        public UpdateReportsSentAtUseCase(IWeekGateway weekGateway, IDbSaver dbSaver)
        {
            _weekGateway = weekGateway;
            _dbSaver = dbSaver;
        }

        public async Task ExecuteAsync(string weekId)
        {
            var week = await _weekGateway.GetWeekAsync(weekId);

            if (week is null) ThrowHelper.ThrowNotFound($"Week not found for: {weekId}");

            if (week.ReportsSentAt is null)
            {
                week.ReportsSentAt = DateTime.UtcNow;
                await _dbSaver.SaveChangesAsync();
            }
        }
    }
}
