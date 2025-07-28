using InventoryApp.Interfaces;
using InventoryApp.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text.Json;

namespace InventoryApp.Services
{
    internal class FileService<T> : IFileService<T>
    {
        private readonly Microsoft.Extensions.Logging.ILogger Logger;
        private void CreateDirectory(string path)
        {
            Logger.LogInformation($"Entering CreateDirectory method");

            var directoryName = Path.GetDirectoryName(path);
            if (directoryName is not null && !Directory.Exists(directoryName))
            {
                Logger.LogInformation($"ُCreate Direcorty : {directoryName}");
                Directory.CreateDirectory(directoryName);
            }
            else
            {
                Logger.LogInformation($"Directory already exists or path is null: {directoryName}");
            }

            Logger.LogInformation("Exiting CreateDirectory method.");
        }
        public FileService()
        {
            var loggerFactory = LoggerFactory.Create(config =>
            {
                config.AddSerilog();
            });
            Logger = loggerFactory.CreateLogger<FileService<T>>();
        }

        public void WriteToFile(T value, string filePath)
        {
            try
            {
                Logger.LogInformation($"Writing data to file at {filePath}");
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    Logger.LogWarning($"File path ({filePath}) must not be null or empty.");
                    throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
                }
                CreateDirectory(filePath);
                var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
                using FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamWriter writer = new StreamWriter(fileStream);

                fileStream.SetLength(0); 
                fileStream.Position = 0;
                writer.Write(json);

                Logger.LogInformation("Successfully wrote data to file.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to write data to file at {filePath}");
                throw;
            }
        }

        public T ReadFromFile(string filePath)
        {
            try
            {
                Logger.LogInformation($"Reading data from file at {filePath}");
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    Logger.LogWarning($"File path ({filePath}) must not be null or empty.");
                    throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
                }
                using FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamReader reader = new StreamReader(fileStream);

                fileStream.Position = 0;
                var content = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(content))
                {
                    Logger.LogWarning("File is empty. Returning empty data.");

                    return Activator.CreateInstance<T>() ?? throw new InvalidOperationException("Failed to create default instance of type.");
                }

                var value = JsonSerializer.Deserialize<T>(content);
                if (value is null)
                {
                    throw new InvalidDataException("Failed to deserialize data from file: content is not in valid format.");
                }

                Logger.LogInformation($"Successfully read data from file.");
                return value;
            }
            catch (JsonException ex)
            {
                Logger.LogError(ex, "File content is not valid JSON.");
                throw new InvalidDataException("File content is not valid JSON.", ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to read data from file at {filePath}");
                throw;
            }
        }
    }
}
