using BonusCalcApi.V1.Boundary.Response;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IOperativesGateway
    {
        Task<OperativeResponse> Execute(string payrollNumber);
    }
}
