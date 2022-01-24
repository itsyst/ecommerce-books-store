using System.ComponentModel.DataAnnotations;

namespace Books.Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Company")]
        public string? Name { get; set; }

        [Display(Name = "Street Adress")]
        public string? StreetAddress { get; set; }

        public string? City { get; set; }
        public string? State { get; set; }

        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
    }
}
