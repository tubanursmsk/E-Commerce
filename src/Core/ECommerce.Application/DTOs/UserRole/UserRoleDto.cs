using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.UserRole
{
    public class UserRoleDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        // BaseEntity
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class UserRoleCreateDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoleId { get; set; }
    }

    public class UserRoleUpdateDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        public bool Status { get; set; } = true;
    }

    public class UserRoleDeleteDto
    {
        [Required]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; } = true;
    }
}
