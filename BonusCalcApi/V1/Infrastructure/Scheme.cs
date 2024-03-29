using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class Scheme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Type { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public decimal ConversionFactor { get; set; }

        public decimal MinValue { get; set; }

        public decimal MaxValue { get; set; }

        public List<PayBand> PayBands { get; set; }
    }
}
