using BonusCalcApi.V1.Infrastructure;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetPayElementTypeUseCase
    {
        Task<PayElementType> ExecuteAsync(string operativePayrollNumber);
    }
}
