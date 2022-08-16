using System;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Controllers.Helpers;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class UpdateBandChangeReportSentAtUseCase : IUpdateBandChangeReportSentAtUseCase
    {
        private readonly IOperativeHelpers _operativeHelpers;
        private readonly IBonusPeriodGateway _bonusPeriodGateway;
        private readonly IBandChangeGateway _bandChangeGateway;
        private readonly IDbSaver _dbSaver;

        public UpdateBandChangeReportSentAtUseCase(
            IOperativeHelpers operativeHelpers,
            IBonusPeriodGateway bonusPeriodGateway,
            IBandChangeGateway bandChangeGateway,
            IDbSaver dbSaver
        )
        {
            _operativeHelpers = operativeHelpers;
            _bonusPeriodGateway = bonusPeriodGateway;
            _bandChangeGateway = bandChangeGateway;
            _dbSaver = dbSaver;
        }

        public async Task ExecuteAsync(string operativeId)
        {
            if (!_operativeHelpers.IsValidPrn(operativeId))
            {
                throw new BadRequestException($"Operative payroll number is invalid");
            }

            var bonusPeriod = await _bonusPeriodGateway.GetEarliestOpenBonusPeriodAsync();

            if (bonusPeriod is null)
            {
                throw new ResourceNotFoundException($"There is no open bonus period");
            }

            if (bonusPeriod.IsClosed)
            {
                throw new ResourceNotProcessableException($"Bonus period has already been closed");
            }

            if (bonusPeriod.Weeks.Any(w => w.IsOpen))
            {
                throw new ResourceNotProcessableException($"Bonus period still has open weeks");
            }

            var bandChange = await _bandChangeGateway.GetBandChangeAsync(bonusPeriod.Id, operativeId);

            if (bandChange is null)
            {
                throw new ResourceNotFoundException($"Band change not found");
            }
            else if (bandChange.FinalBand is null)
            {
                throw new ResourceNotProcessableException($"Band change has not been finalised");
            }
            else if (bandChange.ReportSentAt is null)
            {
                bandChange.ReportSentAt = DateTime.UtcNow;
                await _dbSaver.SaveChangesAsync();
            }
        }
    }
}
