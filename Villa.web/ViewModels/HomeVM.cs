namespace Villla.Web.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Villla.Domain.Entities.Villa> VillaList { get; set; }
        public DateOnly CheckOutDate {  get; set; }
        public DateOnly? CheckInDate { get; set; }
        public int Nights {  get; set; }

    }
}
