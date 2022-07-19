using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Helpers
{
    public interface IBandDecisionHelpers
    {
        public void ValidateBonusPeriod(BonusPeriod bonusPeriod);
        public void ValidateBandChange(BandChange bandChange);
    }
}
