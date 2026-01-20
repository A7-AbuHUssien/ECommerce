using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerce.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public string? MainImg { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public string SKU { get; set; }
        public double Rate { get; set; }
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        public int BrandId { get; set; }
        [ValidateNever]
        public Brand Brand { get; set; }
        [ValidateNever]
        public List<ProductSubImage>? SubImages { get; set; }
        [ValidateNever]
        public ICollection<ProductColor>? Colors { get; set; }
        
    }
}
