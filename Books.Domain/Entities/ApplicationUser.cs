 using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
 
namespace BulkyBook.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(55)]
        public string FullName { get; set; } = string.Empty;

        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
    }
}

 