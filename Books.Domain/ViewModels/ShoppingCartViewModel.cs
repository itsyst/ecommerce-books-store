using Books.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
