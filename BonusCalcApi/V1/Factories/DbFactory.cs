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
                Id = payElementUpdate.Id.GetValueOrDefault(),
                Address = payElementUpdate.Address,
                Comment = payElementUpdate.Comment,
                Monday = payElementUpdate.Monday,
                Tuesday = payElementUpdate.Tuesday,
                Wednesday = payElementUpdate.Wednesday,
                Thursday = payElementUpdate.Thursday,
                Friday = payElementUpdate.Friday,
                Saturday = payElementUpdate.Saturday,
                Sunday = payElementUpdate.Sunday,
                Duration = payElementUpdate.Duration,
                Value = payElementUpdate.Value,
                WorkOrder = payElementUpdate.WorkOrder,
                TradeCode = payElementUpdate.TradeCode,
                CostCode = payElementUpdate.CostCode,
                ClosedAt = payElementUpdate.ClosedAt,
                PayElementTypeId = payElementUpdate.PayElementTypeId
            };
        }
    }
}
