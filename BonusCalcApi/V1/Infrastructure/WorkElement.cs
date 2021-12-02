using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace BonusCalcApi.V1.Infrastructure
{
    public class WorkElement
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string WorkOrder { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public string OperativeId { get; set; }

        public string OperativeName { get; set; }

        public string WeekId { get; set; }

        public decimal Value { get; set; }

        public DateTime? ClosedAt { get; set; }

        public NpgsqlTsVector SearchVector { get; set; }
    }
}
