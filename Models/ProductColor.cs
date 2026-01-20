using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Models;
[PrimaryKey(nameof(ProductId),nameof(Color))]
public class ProductColor
{
    public string Color { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; }
}