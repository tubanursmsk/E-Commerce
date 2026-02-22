using System.ComponentModel.DataAnnotations;namespace ECommerce.Application.DTOs.Customer;

using System.ComponentModel.DataAnnotations;
public class CustomerDeleteDto
{
    [Required]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; } = true;
}
