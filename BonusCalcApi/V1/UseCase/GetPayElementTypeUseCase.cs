using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BonusCalcApi.V1.UseCase
{
    public class GetPayElementTypeUseCase : IGetPayElementTypeUseCase
    {
        /*public GetPaymentUseCase(IPaymentGateway payment)
        {
            Object = @object;
        }

        public IPaymentGateway Object { get; }*/
        public Task<PayElementType> ExecuteAsync(string operativePayrollNumber)
        {
            throw new NotImplementedException();
        }
    }
}
