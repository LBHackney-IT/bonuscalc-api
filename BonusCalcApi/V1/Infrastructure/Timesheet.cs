using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class Timesheet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(6)]
        public string OperativeId { get; set; }
        public Operative Operative { get; set; }

        public int WeekId { get; set; }
        public Week Week { get; set; }

        public List<PayElement> PayElements { get; set; }
    }
}