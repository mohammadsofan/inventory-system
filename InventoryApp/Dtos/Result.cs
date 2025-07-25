using InventoryApp.Validators;

namespace InventoryApp.Dtos
{
    internal class Result
    {
        public string Message { get; set; } = null!;
        public ProductValidatorResult? ValidationResult { get; set; }
        public bool Success { get; set; }
    }
}
