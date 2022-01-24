using Books.Domain.Entities;
using Books.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.Domain.ViewModels
{
#pragma warning disable CS8618
    public class ShoppingCartViewModel
    {

        public Guid Id { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a valur between 1 and 1000")]
        public int Count { get; set; }

        public string? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        [ValidateNever]

        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }

        public double TotalSum { get; set; } = 0;


    }
}
