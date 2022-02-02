using Books.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(55)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(55)]
        public string LastName { get; set; } = string.Empty;

        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        [ForeignKey("CompanyId")]
        [ValidateNever]
        public Company? Company { get; set; }
        public int? CompanyId { get; set; }
    }
}

