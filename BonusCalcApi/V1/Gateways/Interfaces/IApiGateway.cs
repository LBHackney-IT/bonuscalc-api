using System;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IApiGateway
    {
        Task<ApiResponse<TResponse>> ExecuteRequest<TResponse>(string clientName, Uri url) where TResponse : class;
    }
}
