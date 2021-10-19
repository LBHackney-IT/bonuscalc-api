using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Infrastructure;

namespace BonusCalcApi.V1.Factories
{
    public static class DbFactory
    {
        public static PayElement ToDb(this PayElementUpdate payElementUpdate)
        {
            return new PayElement
            {
                Id = payElementUpdate.Id,
                Address = payElementUpdate.Address,
                Comment = payElementUpdate.Comment,
                Duration = payElementUpdate.Duration,
                Value = payElementUpdate.Value,
                WeekDay = payElementUpdate.WeekDay,
                WorkOrder = payElementUpdate.WorkOrder,
                PayElementTypeId = payElementUpdate.PayElementTypeId
            };
        }
    }
}
