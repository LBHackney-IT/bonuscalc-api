using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Gateways;
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

        public async Task<Operative> ExecuteAsync(string operativePayrollNumber)
        {
            return await _operativeGateway.GetAsync(operativePayrollNumber);
        }
    }
}
