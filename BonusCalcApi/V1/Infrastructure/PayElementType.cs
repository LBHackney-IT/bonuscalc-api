using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class PayElementType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public bool PayAtBand { get; set; }

        public bool Paid { get; set; }

        public bool NonProductive { get; set; }

        public bool Productive { get; set; }

        public bool Adjustment { get; set; }

        public bool OutOfHours { get; set; }

        public bool Overtime { get; set; }

        public bool SickLeave { get; set; }

        public bool Selectable { get; set; }

        public int? SmvPerHour { get; set; }

        [StringLength(5)]
        public string CostCode { get; set; }

        public List<PayElement> PayElements { get; set; }
    }
}
