using Asp.Versioning;

namespace BonusCalcApi.Versioning
{
    public static class ApiVersionExtensions
    {
        public static string GetFormattedApiVersion(this ApiVersion apiVersion)
        {
            return $"v{apiVersion}";
        }
    }
}
