namespace BonusCalcApi.V1.Controllers.Helpers
{
    public interface IOperativeHelpers
    {
        public bool IsValidPrn(string prn);
        public bool IsValidDate(string isodate);
    }
}
