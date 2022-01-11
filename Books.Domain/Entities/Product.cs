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

        public string Description { get; set; }

        [Required]
        public Author Author { get; set; }


        [Required]
        [Range(1,10000)]
        public double Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public int CoverId { get; set; }
        public Cover? Cover { get; set; }

        public ICollection<Copy> Copies { get; set; }

    }
}
