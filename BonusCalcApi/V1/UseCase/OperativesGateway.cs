using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetClaimantByIdUseCase
    public class OperativesGateway : IOperativesGateway
    {
        private IExampleGateway _gateway;
        public OperativesGateway(IExampleGateway gateway)
        {
            _gateway = gateway;
        }

        //TODO: rename id to the name of the identifier that will be used for this API, the type may also need to change
        public OperativeResponse Execute(int id)
        {
            return _gateway.GetEntityById(id).ToResponse();
        }
    }
}
