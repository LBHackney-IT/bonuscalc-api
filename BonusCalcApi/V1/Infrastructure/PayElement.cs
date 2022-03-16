using System;
using System.ComponentModel.DataAnnotations;
using BonusCalcApi.V1.Boundary.Request;
using NpgsqlTypes;

namespace BonusCalcApi.V1.Infrastructure
{
    public class PayElement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(17)]
        public string TimesheetId { get; set; }
        public Timesheet Timesheet { get; set; }

        public int PayElementTypeId { get; set; }
        public PayElementType PayElementType { get; set; }

        [StringLength(10)]
        public string WorkOrder { get; set; }

        [StringLength(5)]
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
        public bool ReadOnly { get; set; }
        public DateTime? ClosedAt { get; set; }

        public NpgsqlTsVector SearchVector { get; set; }

        public void UpdateFrom(PayElementUpdate payElement)
        {
            Address = payElement.Address;
            Comment = payElement.Comment;
            Monday = payElement.Monday;
            Tuesday = payElement.Tuesday;
            Wednesday = payElement.Wednesday;
            Thursday = payElement.Thursday;
            Friday = payElement.Friday;
            Saturday = payElement.Saturday;
            Sunday = payElement.Sunday;
            Duration = payElement.Duration;
            Value = payElement.Value;
            WorkOrder = payElement.WorkOrder;
            CostCode = payElement.CostCode;
            PayElementTypeId = payElement.PayElementTypeId;
            ClosedAt = payElement.ClosedAt;
        }
    }
}
