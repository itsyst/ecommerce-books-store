using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Books.Domain.Entities
{
    [Table("Authors")]
    public class Author
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        [MaxLength(255)]
        public string? FullName { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
