using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IOperativeGateway
    {
        public Task<Operative> GetOperativeAsync(string operativeId);
        public Task<IEnumerable<Operative>> GetOperativesAsync(string query, int? page, int? size);
    }
}
