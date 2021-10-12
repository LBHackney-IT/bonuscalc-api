using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways
{
    public interface IOperativeGateway
    {
        Task<Operative> GetAsync(string operativePayrollNumber);
    }
}