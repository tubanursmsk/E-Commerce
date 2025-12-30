using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Product;

public class ProductDeleteDto
{
    public Guid Id { get; set; }
     public bool IsDeleted { get; set; } = true;

}