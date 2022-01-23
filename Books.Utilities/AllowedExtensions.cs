using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Books.Utilities
{
#pragma warning disable CS8603
#pragma warning disable CS8765
    public class AllowedExtensions : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensions(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public static string GetErrorMessage()
        {
            return $"This photo extension is not allowed!";
        }
    }
}
