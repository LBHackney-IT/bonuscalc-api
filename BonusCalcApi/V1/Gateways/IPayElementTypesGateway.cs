using BonusCalcApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.Gateways
{
    public interface IPayElementTypesGateway
    {
        public Task<IEnumerable<PayElementType>> GetPayElementTypesAsync();
    }
}
