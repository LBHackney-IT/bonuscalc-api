using BonusCalcApi.V1.Boundary.Response;
using Microsoft.Extensions.Logging;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.UseCase.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using BonusCalcApi.V1.Exceptions;

namespace BonusCalcApi.V1.Gateways
{

    public class OperativesGateway : IOperativesGateway
    {
        private readonly IApiGateway _apiGateway;
        private readonly ILogger<OperativesGateway> _logger;
        private readonly OperativesGatewayOptions _gatewayOptions;

        public OperativesGateway(IApiGateway apiGateway, ILogger<OperativesGateway> logger, IOptions<OperativesGatewayOptions> gatewayOptions)
        {
            _apiGateway = apiGateway;
            _logger = logger;
            _gatewayOptions = gatewayOptions.Value;
        }

        public async Task<OperativeResponse> Execute(string payrollNumber)
        {
            _logger.LogInformation($"Starting call to RepairsHub for operative [{payrollNumber}]");

            Uri url = new Uri(_gatewayOptions.RepairsHubBaseAddr, UriKind.Absolute);
            var response = await _apiGateway.ExecuteRequest<OperativeResponse>("Repairs", url).ConfigureAwait(false);

            if (response.Status == HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Call to RepairsHub didn't find any operatives for [{payrollNumber}]");
                return null;
            }

            if (!response.IsSuccess)
            {
                _logger.LogError($"Call to RepairsHub failed for [{payrollNumber}]", response.Status);
                throw new ApiException((int) response.Status, "Unable to find operative");
            }

            return response.Content;
        }
    }
}
