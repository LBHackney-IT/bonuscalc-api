using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Response;

namespace BonusCalcApi.V1.Gateways.Interfaces
{
    public interface IOperativesGateway
    {
        Task<OperativeResponse> Execute(string payrollNumber);
    }
}
