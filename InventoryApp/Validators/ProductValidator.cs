using InventoryApp.Models;
namespace InventoryApp.Validators
{
    public class ProductValidatorResult
    {
        public bool IsValid { get; set; }
        public IList<Error> Errors { get; set; } = new List<Error>();
    }
    public class Error
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
    internal class ProductValidator
    {

        public ProductValidatorResult Validate(Product product)
        {
            bool isValid = true;
            IList<Error> errors = new List<Error>();
            if(product.Name.Length < 3)
            {
                errors.Add(new Error()
                {
                    Field = nameof(product.Name),
                    Message = "Name length must be 3 or more characters."
                });
                isValid = false;
            }
            if(product.Description.Length < 3)
            {
                errors.Add(new Error()
                {
                    Field = nameof(product.Description),
                    Message = "Description length must be 3 or more characters."
                });
                isValid = false;
            }
            if (product.Discount < 0 || product.Discount > 1)
            {
                errors.Add(new Error()
                {
                    Field = nameof(product.Discount),
                    Message = "Discount must be between 0.00 and 1.00."
                });
                isValid = false;
            }
            return new ProductValidatorResult()
            {
                IsValid = isValid,
                Errors = errors
            };
        }
    }
}
