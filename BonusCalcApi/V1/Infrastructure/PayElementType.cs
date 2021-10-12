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

        public List<PayElement> PayElements { get; set; }
    }
}