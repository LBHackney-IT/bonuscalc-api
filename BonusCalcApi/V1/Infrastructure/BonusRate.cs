using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class BonusRate
    {
        [Key]
        [StringLength(3)]
        public string Id { get; set; }

        [Required]
        public List<decimal> Rates { get; set; }
    }
}
