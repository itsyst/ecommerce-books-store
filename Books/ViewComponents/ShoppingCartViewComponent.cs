using Books.Domain.ViewModels;
using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;



namespace Books.ViewComponents
{
#pragma warning disable
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork<ShoppingCart> _shoppingCart;
 
        public ShoppingCartViewComponent(IUnitOfWork<ShoppingCart> shoppingCart)
        {
            _shoppingCart = shoppingCart;
         }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            Claim claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                return View(0);

            var carts = await _shoppingCart.Entity.GetAllAsync(c => c.ApplicationUserId == claim.Value, includeProperties: p => p.Product);
             
            var items = 0;
            foreach (var cart in carts)
            {
                items += cart.Count;
            }

            return View(items);
        }
    }
}
