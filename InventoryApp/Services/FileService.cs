using InventoryApp.Interfaces;
using InventoryApp.Models;
using System.Text.Json;


namespace InventoryApp.Services
{
    internal class FileService : IFileService
    {
        private readonly string FilePath;
        public FileService(string path)
        {
            FilePath = path;
            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            }
        }
        public void WriteToFile(IList<Product> products)
        {
            try
            {
                var json = JsonSerializer.Serialize(products);
                using FileStream fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamWriter writer = new StreamWriter(fileStream);
                fileStream.SetLength(0);
                fileStream.Position = 0;
                writer.Write(json);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<Product> ReadFromFile()
        {
            try
            {
                using FileStream fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamReader reader = new StreamReader(fileStream);
                fileStream.Position = 0;
                var content = reader.ReadToEnd();
                if (String.IsNullOrWhiteSpace(content))
                {
                    return new List<Product>();
                }
                var products = JsonSerializer.Deserialize<IList<Product>>(content);
                if(products is null)
                {
                    throw new InvalidDataException("Failed to deserialize products list from file: content is not in valid format.");
                }
                return products;
            }
            catch (Exception ex) when (ex is JsonException)
            {
                throw new InvalidDataException("File content is not valid JSON.", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
