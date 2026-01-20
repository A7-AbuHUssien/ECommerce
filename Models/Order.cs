using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    public Guid OrderGuid { get; set; } = Guid.NewGuid();
    [Required]
    public string UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string OrderStatus { get; set; } = "Processing";

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }
    public string PaymentStatus  { get; set; }
    [Required]
    public string StripeSessionId { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
