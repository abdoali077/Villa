namespace Villla.Web.ViewModels
{
    public class RadialBarChartVM
    {
        public int TotalCount {  get; set; }
        public decimal IncreaseDecreaseAmount { get; set; }
        public bool HasRatioIncreases { get; set; }
        public decimal[] Series { get; set; }

        public int BookingsDifference { get; set; }
    }
}
