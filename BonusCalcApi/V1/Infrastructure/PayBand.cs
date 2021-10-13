using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class PayBand
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        public string TradeId { get; set; }
        public Trade Trade { get; set; }

        public int Band { get; set; }

        public decimal Value { get; set; }
    }
}
