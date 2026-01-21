namespace ECommerce.ViewModels;

public class ProductFilterVM
{
    public string ProductName { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal BrandId { get; set; }
    public int CategoryId { get; set; }
    public int QuantityUnder { get; set; }
}