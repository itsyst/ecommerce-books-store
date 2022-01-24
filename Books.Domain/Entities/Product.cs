using Books.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Books.Domain.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(55)]
        public string? Title { get; set; }


        [Required]
        public string? ISBN { get; set; }

        public string? Description { get; set; }

        [Required]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }
        [ValidateNever]
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
        [ValidateNever]
        public Category? Category { get; set; }

        [Required]
        [Display(Name = "Cover")]
        public int CoverId { get; set; }
        [ValidateNever]
        public Cover? Cover { get; set; }

        [Range(0, 100)]
        public int InStock { get; set; }

    }
}
