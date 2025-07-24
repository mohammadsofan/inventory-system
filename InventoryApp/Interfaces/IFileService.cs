using InventoryApp.Models;


namespace InventoryApp.Interfaces
{
    internal interface IFileService
    {
        void WriteToFile(IList<Product> products);
        IList<Product> ReadFromFile();
    }
}
