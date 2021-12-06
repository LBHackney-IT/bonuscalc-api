using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetWorkElementsUseCase
    {
        public Task<IEnumerable<WorkElement>> ExecuteAsync(string query, int? page, int? size);
    }
}
