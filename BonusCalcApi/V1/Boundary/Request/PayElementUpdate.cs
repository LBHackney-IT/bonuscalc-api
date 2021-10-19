namespace BonusCalcApi.V1.Boundary.Request
{
    public class PayElementUpdate
    {
        public int Id { get; set; }

        public int PayElementTypeId { get; set; }

        public int WeekDay { get; set; }

        public string WorkOrder { get; set; }

        public string Address { get; set; }

        public string Comment { get; set; }

        public bool Productive { get; set; }

        public decimal Duration { get; set; }

        public decimal Value { get; set; }
    }
}
