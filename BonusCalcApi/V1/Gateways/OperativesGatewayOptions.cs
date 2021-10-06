using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Gateways
{
    public class OperativesGatewayOptions
    {
        public const string OpGatewayOptionsName = "OperativesGatewayOptions";
        [Required]
        public string RepairsHubBaseAddr { get; set; }
        [Required]
        public string RepairsHubApiKey { get; set; }
    }
}
