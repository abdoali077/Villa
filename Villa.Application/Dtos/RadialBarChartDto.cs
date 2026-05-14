namespace Villla.Application.Dtos
{
    public class RadialBarChartDto
    {
        public decimal TotalCount {  get; set; }
        public decimal IncreaseDecreaseAmount { get; set; }
        public bool HasRatioIncreases { get; set; }
        public decimal[] Series { get; set; }

        public decimal Difference { get; set; }
    }
}
