using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetOperativeUseCase
    {
        Task<Operative> ExecuteAsync(string operativePayrollNumber);
    }
}
