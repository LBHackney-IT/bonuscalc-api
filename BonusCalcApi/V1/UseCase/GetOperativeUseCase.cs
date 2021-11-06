using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetOperativeUseCase : IGetOperativeUseCase
    {
        private readonly IOperativeGateway _operativeGateway;

        public GetOperativeUseCase(IOperativeGateway operativeGateway)
        {
            _operativeGateway = operativeGateway;
        }

        public async Task<Operative> ExecuteAsync(string operativeId)
        {
            return await _operativeGateway.GetAsync(operativeId);
        }
    }
}
