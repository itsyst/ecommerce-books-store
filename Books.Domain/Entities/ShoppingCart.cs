using Books.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Books.Domain.Entities
{
#pragma warning disable CS8618
    public class ShoppingCart
    {

        public Guid Id { get; set; }


        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }

        public int ProductId { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }

        public string? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        public double PriceHolder { get; set; }

        [NotMapped]
        private double _priceHolderRabat;

        public double PriceHolderRabat()
        {
            return _priceHolderRabat;
        }

        public double SetPriceHolderRabat(double priceHolderRabat)
        {
            _priceHolderRabat = priceHolderRabat;
            return _priceHolderRabat;
        }

    }
}
