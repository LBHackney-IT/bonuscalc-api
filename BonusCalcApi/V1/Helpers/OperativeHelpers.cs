using System.Text.RegularExpressions;

namespace BonusCalcApi.V1.Controllers.Helpers
{
    public class OperativeHelpers : IOperativeHelpers
    {
        private static readonly Regex _prnMatcher = new Regex("^[0-9]{6}$");
        private static readonly Regex _dateMatcher = new Regex("^[0-9]{4}-[0-9]{2}-[0-9]{2}$");

        public bool IsValidPrn(string prn)
        {
            return !string.IsNullOrWhiteSpace(prn) && _prnMatcher.IsMatch(prn);
        }

        public bool IsValidDate(string isodate)
        {
            return !string.IsNullOrWhiteSpace(isodate) && _dateMatcher.IsMatch(isodate);
        }
    }
}
