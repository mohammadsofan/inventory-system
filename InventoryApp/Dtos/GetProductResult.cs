using InventoryApp.Models;

namespace InventoryApp.Dtos
{
    internal class GetProductResult
    {
        public Product? Product { get; set; }
        public string? Message {  get; set; }
    }
}
