using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IOperativeGateway
    {
        public Task<Operative> GetAsync(string operativeId);
    }
}
