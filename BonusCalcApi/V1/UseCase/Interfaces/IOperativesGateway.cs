using BonusCalcApi.V1.Boundary.Response;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IOperativesGateway
    {
        OperativeResponse Execute(string payrollNumber);
    }
}
