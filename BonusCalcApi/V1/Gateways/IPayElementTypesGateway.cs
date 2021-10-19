using BonusCalcApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.Gateways
{
    public interface IPayElementTypesGateway
    {
        public Task<PayElementType> GetPayTypesAsync(string payElementId);
    }
}
