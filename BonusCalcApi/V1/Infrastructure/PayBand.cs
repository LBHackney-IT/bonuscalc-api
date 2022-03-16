using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class PayBand
    {
        [Key]
        public int Id { get; set; }

        public int? SchemeId { get; set; }
        public Scheme Scheme { get; set; }

        public int Band { get; set; }

        public decimal Value { get; set; }

        public decimal TotalValue { get; set; }

        public decimal SmvPerHour { get; set; }
    }
}
