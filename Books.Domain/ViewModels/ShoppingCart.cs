using Books.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Books.Domain.ViewModels
{
    public class ShoppingCart
    {
        public Product? Product { get; set; }

        [Range(1,1000, ErrorMessage="Please enter a valur between 1 and 1000")]
        public int Count { get; set; }
    }
}
