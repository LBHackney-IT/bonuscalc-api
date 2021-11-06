using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetPayElementTypeUseCase
    {
        public Task<IEnumerable<PayElementType>> ExecuteAsync();
    }
}
