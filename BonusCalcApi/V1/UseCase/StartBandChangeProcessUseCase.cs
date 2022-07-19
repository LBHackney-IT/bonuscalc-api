using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class StartBandChangeProcessUseCase : IStartBandChangeProcessUseCase
    {
        private readonly IBonusPeriodGateway _bonusPeriodGateway;
        private readonly IOperativeProjectionGateway _operativeProjectionGateway;
        private readonly IDbSaver _dbSaver;

        public StartBandChangeProcessUseCase(
            IBonusPeriodGateway bonusPeriodGateway,
            IOperativeProjectionGateway operativeProjectionGateway,
            IDbSaver dbSaver)
        {
            _bonusPeriodGateway = bonusPeriodGateway;
            _operativeProjectionGateway = operativeProjectionGateway;
            _dbSaver = dbSaver;
        }

        public async Task<BonusPeriod> ExecuteAsync()
        {
            var bonusPeriod = await _bonusPeriodGateway.GetEarliestOpenBonusPeriodAsync();

            if (bonusPeriod is null)
            {
                throw new ResourceNotFoundException($"Open bonus period not found");
            }

            if (bonusPeriod.IsClosed)
            {
                throw new ResourceNotProcessableException($"Bonus period has already been closed");
            }

            if (bonusPeriod.BandChanges.Count > 0)
            {
                throw new ResourceNotProcessableException($"Bonus period already has band changes");
            }

            if (bonusPeriod.Weeks.Any(w => w.IsOpen))
            {
                throw new ResourceNotProcessableException($"Bonus period still has open weeks");
            }

            var projections = await _operativeProjectionGateway.GetAllByBonusPeriodIdAsync(bonusPeriod.Id);
            var bandChanges = projections.Select(p => new BandChange(p));

            bonusPeriod.BandChanges ??= new List<BandChange>();
            bonusPeriod.BandChanges.AddRange(bandChanges);

            await _dbSaver.SaveChangesAsync();

            return bonusPeriod;
        }
    }
}
