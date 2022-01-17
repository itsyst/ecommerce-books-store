using Books.Domain.Entities;
using Books.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Books.Dtos
{
    public class ProductDto
    {

        [Required]
        [MaxLength(55)]
        public string? Title { get; set; }


        [Required]
        public string? ISBN { get; set; }

        public string? Description { get; set; }

        [Required]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }
        public Author? Author { get; set; }


        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }

        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "Maximum allowed file size is 5 MB")]
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg", ".svg" })]
        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        [Display(Name = "Cover")]
        public int CoverId { get; set; }
        public Cover? Cover { get; set; }

        [Range(0, 100)]
        public int InStock { get; set; }
    }
}
