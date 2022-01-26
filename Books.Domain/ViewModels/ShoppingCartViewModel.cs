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
        [ValidateNever]
        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }

        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }


    }
}
