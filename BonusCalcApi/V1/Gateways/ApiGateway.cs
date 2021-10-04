using BonusCalcApi.V1.Gateways;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RepairsApi.V2.Gateways
{
#nullable enable
    public class ApiGateway : IApiGateway
    {
        private readonly IHttpClientFactory _clientFactory;

        public ApiGateway(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<ApiResponse<TResponse>> ExecuteRequest<TResponse>(string clientName, Uri url)
            where TResponse : class
        {
            var client = _clientFactory.CreateClient(clientName);
            TResponse? response = default;

            var result = await client.GetAsync(url).ConfigureAwait(true);

            if (result.IsSuccessStatusCode)
            {
                var stringResult = await result.Content.ReadAsStringAsync().ConfigureAwait(true);

                response = JsonConvert.DeserializeObject<TResponse>(stringResult);
            }
            return new ApiResponse<TResponse>(result.IsSuccessStatusCode, result.StatusCode, response);
        }
    }
}
