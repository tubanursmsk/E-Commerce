using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Cargo;

public class CargoDeleteDto
{
     [Required]
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; } = true;
}