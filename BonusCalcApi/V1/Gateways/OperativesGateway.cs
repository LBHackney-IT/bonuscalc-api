using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    
    public class OperativesGateway : IOperativesGateway
    {
        public OperativesGateway()
        {
            
        }

        public OperativeResponse Execute(string payrollNumber)
        {
            return new OperativeResponse();
        }
    }
}
