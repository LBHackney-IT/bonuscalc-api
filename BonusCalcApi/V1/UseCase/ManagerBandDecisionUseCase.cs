using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Helpers;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class ManagerBandDecisionUseCase : IManagerBandDecisionUseCase
    {
        private readonly IBandDecisionHelpers _helpers;
        private readonly IBonusPeriodGateway _bonusPeriodGateway;
        private readonly IBandChangeGateway _bandChangeGateway;
        private readonly IDbSaver _dbSaver;

        public ManagerBandDecisionUseCase(
            IBonusPeriodGateway bonusPeriodGateway,
            IBandChangeGateway bandChangeGateway,
            IDbSaver dbSaver)
        {
            _helpers = new BandDecisionHelpers();
            _bonusPeriodGateway = bonusPeriodGateway;
            _bandChangeGateway = bandChangeGateway;
            _dbSaver = dbSaver;
        }

        public async Task<BandChange> ExecuteAsync(string operativeId, BandChangeRequest request)
        {
            var bonusPeriod = await _bonusPeriodGateway.GetEarliestOpenBonusPeriodAsync();
            _helpers.ValidateBonusPeriod(bonusPeriod);

            var bandChange = await _bandChangeGateway.GetBandChangeAsync(bonusPeriod.Id, operativeId);
            _helpers.ValidateBandChange(bandChange);

            if (bandChange.Supervisor.Decision == null)
            {
                throw new ResourceNotProcessableException($"Band change has not been processed by the supervisor yet");
            }

            bandChange.Manager = new BandChangeApprover
            {
                Name = request.Name,
                EmailAddress = request.EmailAddress,
                Decision = request.Decision,
                Reason = request.Reason,
                SalaryBand = request.SalaryBand
            };

            bandChange.FinalBand = request.SalaryBand;

            await _dbSaver.SaveChangesAsync();

            return bandChange;
        }
    }
}
