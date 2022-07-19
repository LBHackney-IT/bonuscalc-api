using System.Linq;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Helpers
{
    public class BandDecisionHelpers : IBandDecisionHelpers
    {
        public void ValidateBonusPeriod(BonusPeriod bonusPeriod)
        {
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
        }

        public void ValidateBandChange(BandChange bandChange)
        {
            if (bandChange is null)
            {
                throw new ResourceNotFoundException($"Band change not found");
            }
        }
    }
}
