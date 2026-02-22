using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Order;

public class OrderDeleteDto
{
      [Required]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; } = true;
    }