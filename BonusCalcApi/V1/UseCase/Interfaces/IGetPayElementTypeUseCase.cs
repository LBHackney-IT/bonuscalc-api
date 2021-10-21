using BonusCalcApi.V1.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetPayElementTypeUseCase
    {
        Task<IEnumerable<PayElementType>> ExecuteAsync();
    }
}
