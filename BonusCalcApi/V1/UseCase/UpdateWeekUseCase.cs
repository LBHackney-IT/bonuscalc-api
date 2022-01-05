using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class UpdateWeekUseCase : IUpdateWeekUseCase
    {
        private readonly IWeekGateway _weekGateway;
        private readonly IDbSaver _dbSaver;

        public UpdateWeekUseCase(IWeekGateway weekGateway, IDbSaver dbSaver)
        {
            _weekGateway = weekGateway;
            _dbSaver = dbSaver;
        }

        public async Task<Week> ExecuteAsync(string weekId, WeekUpdate request)
        {
            var week = await _weekGateway.GetWeekAsync(weekId);

            if (week is null) return null;

            if (week.ClosedAt is null)
            {
                if (!(request.ClosedAt is null))
                {
                    week.ClosedAt = request.ClosedAt;
                    week.ClosedBy = request.ClosedBy;
                    await _dbSaver.SaveChangesAsync();
                }
            }
            else
            {
                if (request.ClosedAt is null)
                {
                    week.ClosedAt = null;
                    week.ClosedBy = null;
                    await _dbSaver.SaveChangesAsync();
                }
            }

            return week;
        }
    }
}
