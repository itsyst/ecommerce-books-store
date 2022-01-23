using System.ComponentModel.DataAnnotations;

namespace Books.Domain.Entities
{
# nullable disable
    public class CategoryDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100 !")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDateTime { get; set; } = DateTime.Now;

    }
}
