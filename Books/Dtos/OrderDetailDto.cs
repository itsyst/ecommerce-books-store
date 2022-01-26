using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace Books.Domain.Entities
{
    [Table("OrderDetail")]
    public class OrderDetailDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        [ValidateNever]
        public OrderHeader? OrderHeader { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }
    }
}
