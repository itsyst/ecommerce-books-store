using System.ComponentModel.DataAnnotations;

namespace Books.Domain.Entities
{
    public class Cover
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(55)]
        public string? Name { get; set; }    
    }
}
