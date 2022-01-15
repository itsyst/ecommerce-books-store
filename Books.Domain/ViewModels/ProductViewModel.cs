using Books.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Books.Domain.ViewModels
{
#pragma warning disable CS8618 

    public class ProductViewModel
    {
        public Product Product { get; set; }

        [ValidateNever]
        [Display(Name = "Author")]
        public IEnumerable<SelectListItem> Authors { get; set; }

        [ValidateNever]
        [Display(Name = "Category")]
        public IEnumerable<SelectListItem> Categories { get; set; }

        [ValidateNever]
        [Display(Name = "Cover")]
        public IEnumerable<SelectListItem> Covers { get; set; }

    }
}
