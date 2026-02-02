using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Domain.Entities;

public class ProductImage : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; } // Ana resim mi?

    // Product ile ilişki
    public Guid ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;
}