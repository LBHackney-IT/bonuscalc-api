using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IWorkElementGateway
    {
        public Task<IEnumerable<WorkElement>> GetWorkElementsAsync(string query, int? page, int? size);
    }
}
