using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.DTOs.Company;

    public class CompanyUpdateDto
    {
        [Required, MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [Required, MinLength(5)]
        public string FullAddress { get; set; } = string.Empty;

        [Required, MinLength(10)]
        public string Phone { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }

  