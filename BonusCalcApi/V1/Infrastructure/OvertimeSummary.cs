namespace BonusCalcApi.V1.Infrastructure
{
    public class OvertimeSummary
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string WeekId { get; set; }

        public string TradeId { get; set; }

        public string TradeDescription { get; set; }

        public string CostCode { get; set; }

        public decimal TotalValue { get; set; }
    }
}
