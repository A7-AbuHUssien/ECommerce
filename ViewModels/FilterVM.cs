namespace ECommerce.ViewModels
{
    public class FilterVM
    {
        public string ProductName { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int CategoryId { get; set; }
        public bool IsHot { get; set; }
    }
}
