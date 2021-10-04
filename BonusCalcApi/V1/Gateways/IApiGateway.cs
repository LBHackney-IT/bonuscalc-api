using System;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.Gateways
{
    public interface IApiGateway
    {
        Task<ApiResponse<TResponse>> ExecuteRequest<TResponse>(string clientName, Uri url) where TResponse : class;
    }
}
