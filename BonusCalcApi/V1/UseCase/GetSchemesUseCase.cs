using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetSchemesUseCase : IGetSchemesUseCase
    {
        private readonly ISchemeGateway _schemeGateway;

        public GetSchemesUseCase(ISchemeGateway schemeGateway)
        {
            _schemeGateway = schemeGateway;
        }

        public async Task<IEnumerable<Scheme>> ExecuteAsync()
        {
            return await _schemeGateway.GetSchemesAsync();
        }
    }
}
