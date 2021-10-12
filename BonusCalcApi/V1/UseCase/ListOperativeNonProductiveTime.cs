using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.V1.UseCase
{
    public class ListOperativeNonProductiveTime : IListOperativeNonProductiveTime
    {
        private readonly INonProductiveTimeGateway _nonProductiveTimeGateway;
        public ListOperativeNonProductiveTime(INonProductiveTimeGateway nonProductiveTimeGateway)
        {
            _nonProductiveTimeGateway = nonProductiveTimeGateway;
        }
        public Task<IEnumerable<NonProductiveTime>> Execute(string operativePayrollNumber)
        {
            return _nonProductiveTimeGateway.GetNonProductiveTimeAsync(operativePayrollNumber);
        }
    }

}
