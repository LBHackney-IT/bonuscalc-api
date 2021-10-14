using System.Text.RegularExpressions;

namespace BonusCalcApi.V1.Controllers.Helpers
{
    public class OperativeHelpers : IOperativeHelpers
    {
        private static readonly Regex _prnMatcher = new Regex("^[0-9]{6}$");
        public bool IsValidPrn(string prn)
        {
            return !string.IsNullOrWhiteSpace(prn) && _prnMatcher.IsMatch(prn);
        }
    }
}
