using System;

namespace BonusCalcApi.V1.Boundary.Request
{
    public class PayElementUpdate
    {
        public int? Id { get; set; }

        public int PayElementTypeId { get; set; }

        public string WorkOrder { get; set; }

        public string CostCode { get; set; }

        public string Address { get; set; }

        public string Comment { get; set; }

        public decimal Monday { get; set; }
        public decimal Tuesday { get; set; }
        public decimal Wednesday { get; set; }
        public decimal Thursday { get; set; }
        public decimal Friday { get; set; }
        public decimal Saturday { get; set; }
        public decimal Sunday { get; set; }

        public decimal Duration { get; set; }

        public decimal Value { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}
