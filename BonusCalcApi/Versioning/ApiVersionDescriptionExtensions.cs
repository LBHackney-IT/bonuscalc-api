using Asp.Versioning.ApiExplorer;

namespace BonusCalcApi.Versioning
{
    public static class ApiVersionDescriptionExtensions
    {
        public static string GetFormattedApiVersion(this ApiVersionDescription apiVersionDescription)
        {
            return $"v{apiVersionDescription.ApiVersion}";
        }
    }
}

