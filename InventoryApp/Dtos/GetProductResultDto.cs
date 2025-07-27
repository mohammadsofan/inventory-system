using InventoryApp.Models;

namespace InventoryApp.Dtos
{
    internal class GetProductResultDto
    {
        public Product? Product { get; set; }
        public string? Message {  get; set; }
    }
}
