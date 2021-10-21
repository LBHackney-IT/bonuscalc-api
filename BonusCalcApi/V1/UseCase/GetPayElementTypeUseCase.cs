using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.UseCase
{
    public class GetPayElementTypeUseCase : IGetPayElementTypeUseCase
    {
        private IPayElementTypesGateway _payElementType;

        public GetPayElementTypeUseCase(IPayElementTypesGateway payElementType)
        {
            _payElementType = payElementType;
        }

        public async Task<IEnumerable<PayElementType>> ExecuteAsync()
        {
            return await _payElementType.GetPayElementTypesAsync();
        }
    }
}
