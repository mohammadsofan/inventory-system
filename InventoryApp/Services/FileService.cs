using InventoryApp.Interfaces;
using InventoryApp.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text.Json;

namespace InventoryApp.Services
{
    internal class FileService<T> : IFileService<T> 
    {
        private readonly string FilePath;
        private readonly Microsoft.Extensions.Logging.ILogger Logger;

        public FileService(string path)
        {
            FilePath = path;
            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            }

            var loggerFactory = LoggerFactory.Create(config =>
            {
                config.AddSerilog();
            });
            Logger = loggerFactory.CreateLogger<FileService<T>>();
        }

        public void WriteToFile(T value)
        {
            try
            {
                Logger.LogInformation($"Writing value {value} products to file at {FilePath}");

                var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
                using FileStream fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamWriter writer = new StreamWriter(fileStream);

                fileStream.SetLength(0); 
                fileStream.Position = 0;
                writer.Write(json);

                Logger.LogInformation("Successfully wrote products to file.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to write products to file at {FilePath}");
                throw;
            }
        }

        public T ReadFromFile()
        {
            try
            {
                Logger.LogInformation($"Reading products from file at {FilePath}");

                using FileStream fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamReader reader = new StreamReader(fileStream);

                fileStream.Position = 0;
                var content = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(content))
                {
                    Logger.LogWarning("File is empty. Returning empty product list.");
                    return Activator.CreateInstance<T>() ?? throw new InvalidOperationException("Failed to create default instance of type.");
                }

                var value = JsonSerializer.Deserialize<T>(content);
                if (value is null)
                {
                    throw new InvalidDataException("Failed to deserialize products list from file: content is not in valid format.");
                }

                Logger.LogInformation($"Successfully read value from file.");
                return value;
            }
            catch (JsonException ex)
            {
                Logger.LogError(ex, "File content is not valid JSON.");
                throw new InvalidDataException("File content is not valid JSON.", ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to read products from file at {FilePath}");
                throw;
            }
        }
    }
}
