using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetPayElementTypeUseCase : IGetPayElementTypeUseCase
    {
        private readonly IPayElementTypeGateway _payElementTypesGateway;

        public GetPayElementTypeUseCase(IPayElementTypeGateway payElementTypesGateway)
        {
            _payElementTypesGateway = payElementTypesGateway;
        }

        public async Task<IEnumerable<PayElementType>> ExecuteAsync()
        {
            return await _payElementTypesGateway.GetPayElementTypesAsync();
        }
    }
}
