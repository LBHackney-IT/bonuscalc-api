using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class GetWorkElementsUseCase : IGetWorkElementsUseCase
    {
        private readonly IWorkElementGateway _workElementGateway;

        public GetWorkElementsUseCase(IWorkElementGateway workElementGateway)
        {
            _workElementGateway = workElementGateway;
        }

        public async Task<IEnumerable<WorkElement>> ExecuteAsync(string query, int? page, int? size)
        {
            return await _workElementGateway.GetWorkElementsAsync(query, page, size);
        }
    }
}
