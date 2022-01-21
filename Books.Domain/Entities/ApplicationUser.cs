using Books.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
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
        public Company? Company { get; set; }
        public int CompanyId { get; set; }
    }
}

 