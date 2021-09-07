using BonusCalcApi.V1.Boundary.Response;

namespace BonusCalcApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        ResponseObject Execute(int id);
    }
}
