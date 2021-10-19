using System.ComponentModel.DataAnnotations;
using BonusCalcApi.V1.Boundary.Request;

namespace BonusCalcApi.V1.Infrastructure
{
    public class PayElement
    {
        [Key]
        public int Id { get; set; }

        public int TimesheetId { get; set; }
        public Timesheet Timesheet { get; set; }

        public int PayElementTypeId { get; set; }
        public PayElementType PayElementType { get; set; }

        public int WeekDay { get; set; }

        [StringLength(10)]
        public string WorkOrder { get; set; }

        public string Address { get; set; }

        public string Comment { get; set; }

        public decimal Duration { get; set; }

        public decimal Value { get; set; }

        public void UpdateFrom(PayElementUpdate payElement)
        {
            Address = payElement.Address;
            Comment = payElement.Comment;
            Duration = payElement.Duration;
            Value = payElement.Value;
            WeekDay = payElement.WeekDay;
            WorkOrder = payElement.WorkOrder;
            PayElementTypeId = payElement.PayElementTypeId;
        }
    }
}
