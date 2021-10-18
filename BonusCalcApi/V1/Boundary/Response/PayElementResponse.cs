namespace BonusCalcApi.V1.Boundary.Response
{
    public class PayElementResponse
    {
        public int Id { get; set; }

        public PayElementTypeResponse PayElementType { get; set; }

        public int WeekDay { get; set; }

        public string WorkOrder { get; set; }

        public string Address { get; set; }

        public string Comment { get; set; }

        public bool Productive { get; set; }

        public decimal Duration { get; set; }

        public decimal Value { get; set; }
    }

    public class PayElementTypeResponse
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public bool PayAtBand { get; set; }

        public bool Paid { get; set; }
    }
}
