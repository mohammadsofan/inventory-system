using InventoryApp.Models;


namespace InventoryApp.Interfaces
{
    internal interface IFileService<T>
    {
        void WriteToFile(T value);
        T ReadFromFile();
    }
}
