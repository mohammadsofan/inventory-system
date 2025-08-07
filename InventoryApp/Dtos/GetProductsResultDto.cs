using InventoryApp.Models;

namespace InventoryApp.Dtos
{
    internal class GetProductsResultDto
    {
        public IList<Product> Products { get; set; } = new List<Product>();
        public string Message { get; set; } = string.Empty;
    }
}
