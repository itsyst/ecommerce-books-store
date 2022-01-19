using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Display(Name="Street Adress")]
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
