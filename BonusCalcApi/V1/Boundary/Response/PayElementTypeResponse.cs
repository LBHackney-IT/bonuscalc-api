namespace BonusCalcApi.V1.Boundary.Response
{
    public class PayElementTypeResponse
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public bool PayAtBand { get; set; }

        public bool Paid { get; set; }

        public bool Productive { get; set; }

        public bool Adjustment { get; set; }
    }
}
