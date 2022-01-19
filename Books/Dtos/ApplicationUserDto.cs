using Books.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.Dtos
{
    public class ApplicationUserDto
    {
        [Required]
        [MaxLength(55)]
        public string FullName { get; set; } = string.Empty;

        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
        public int CompanyId { get; set; }
    }
}
