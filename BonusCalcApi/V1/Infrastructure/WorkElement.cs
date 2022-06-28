using System;
using NpgsqlTypes;

namespace BonusCalcApi.V1.Infrastructure
{
    public class WorkElement
    {
        public int Id { get; set; }

        public int PayElementTypeId { get; set; }
        public PayElementType PayElementType { get; set; }

        public string WorkOrder { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public string OperativeId { get; set; }

        public string OperativeName { get; set; }

        public string WeekId { get; set; }
        public Week Week { get; set; }

        public decimal Value { get; set; }

        public DateTime? ClosedAt { get; set; }

        public NpgsqlTsVector SearchVector { get; set; }
    }
}
