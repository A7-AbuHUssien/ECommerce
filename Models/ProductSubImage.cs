using Microsoft.EntityFrameworkCore;

namespace ECommerce.Models
{
    [PrimaryKey(nameof(Img), nameof(ProductId))]
    public class ProductSubImage
    {
        public string Img { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}