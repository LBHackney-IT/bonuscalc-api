using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IPayElementTypeGateway
    {
        public Task<IEnumerable<PayElementType>> GetPayElementTypesAsync();
    }
}
