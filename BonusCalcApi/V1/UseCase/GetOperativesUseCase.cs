using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetOperativesUseCase : IGetOperativesUseCase
    {
        private readonly IOperativeGateway _operativeGateway;

        public GetOperativesUseCase(IOperativeGateway operativeGateway)
        {
            _operativeGateway = operativeGateway;
        }

        public async Task<IEnumerable<Operative>> ExecuteAsync(string query, int? page, int? size)
        {
            return await _operativeGateway.GetOperativesAsync(query, page, size);
        }
    }
}
