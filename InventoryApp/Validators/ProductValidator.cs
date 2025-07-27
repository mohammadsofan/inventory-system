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
            IList<Error> errors = new List<Error>();
            if(product.Name.Length < 3 || product.Name.Length > 20)
            {
                errors.Add(new Error()
                {
                    Field = nameof(product.Name),
                    Message = "Name length must be between 3 and 20 characters."
                });
            }
            if(product.Description.Length < 3 || product.Name.Length > 50)
            {
                errors.Add(new Error()
                {
                    Field = nameof(product.Description),
                    Message = "Description length must be between 3 and 50 characters."
                });
            }
            if (product.Discount < 0 || product.Discount > 1)
            {
                errors.Add(new Error()
                {
                    Field = nameof(product.Discount),
                    Message = "Discount must be between 0.00 and 1.00."
                });
            }
            if(product.Price < 0)
            {
                errors.Add(new Error()
                {
                    Field = nameof(product.Price),
                    Message = "Price cannot be negative value"
                });
            }
            if (product.Quantity < 0)
            {
                errors.Add(new Error()
                {
                    Field = nameof(product.Quantity),
                    Message = "Quantity cannot be negative value"
                });
            }
            return new ProductValidatorResult()
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }
    }
}
