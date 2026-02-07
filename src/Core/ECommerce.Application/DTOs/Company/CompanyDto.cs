using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Company;

public class CompanyDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Company name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full address is required.")]
    public string FullAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tax number is required.")]
    public string TaxNumber { get; set; } = string.Empty;
    
     // Güvenlik: Bunu her yerde göstermeyebilirsin (admin'e özel yapman daha doğru)
    public string ApiKey { get; set; } = string.Empty; // Yönergedeki API Key gereksinimi için

    public bool IsApproved { get; set; }

      // BaseEntity
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
}