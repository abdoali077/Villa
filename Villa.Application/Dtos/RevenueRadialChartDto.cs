namespace Villla.Application.Dtos
{
    public class RevenueRadialChartDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal IncreaseDecreaseAmount { get; set; }
        public bool HasRatioIncreases { get; set; }
        public decimal[] Series { get; set; }
        public decimal Difference { get; set; }
    }
}
