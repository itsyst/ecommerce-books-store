using System.ComponentModel.DataAnnotations;

namespace BooksWebApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Display(Name ="Order")]
        public int DisplayOrder { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

    }
}
