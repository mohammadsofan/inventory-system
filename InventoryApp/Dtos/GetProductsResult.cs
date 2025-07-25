using InventoryApp.Models;

namespace InventoryApp.Dtos
{
    internal class GetProductsResult
    {
        public IList<Product>? Products { get; set; }
        public string? Message { get; set; }
    }
}
