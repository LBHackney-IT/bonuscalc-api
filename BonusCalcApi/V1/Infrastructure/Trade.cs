using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class Trade
    {
        [Key]
        [StringLength(3)]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
