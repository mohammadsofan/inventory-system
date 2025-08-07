using InventoryApp.Interfaces;
using Serilog;
using System.Text.Json;

namespace InventoryApp.Services
{
    internal class FileService<T> : IFileService<T>
    {
        private void CreateDirectory(string path)
        {
            Log.Information($"Entering CreateDirectory method");

            var directoryName = Path.GetDirectoryName(path);
            if (directoryName is not null && !Directory.Exists(directoryName))
            {
                Log.Information("Create Direcorty : {directoryName}",directoryName);
                Directory.CreateDirectory(directoryName);
            }
            else
            {
                Log.Information("Directory already exists or path is null: {directoryName}",directoryName);
            }

            Log.Information("Exiting CreateDirectory method.");
        }

        public void WriteToFile(T value, string filePath)
        {
            try
            {
                Log.Information("Writing data to file at {filePath}",filePath);
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    Log.Warning("File path ({filePath}) must not be null or empty.",filePath);
                    throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
                }
                CreateDirectory(filePath);
                var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
                using FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamWriter writer = new StreamWriter(fileStream);

                fileStream.SetLength(0); 
                fileStream.Position = 0;
                writer.Write(json);

                Log.Information("Successfully wrote data to file.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to write data to file at {filePath}",filePath);
                throw;
            }
        }

        public T ReadFromFile(string filePath)
        {
            try
            {
                Log.Information("Reading data from file at {filePath}", filePath);
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    Log.Warning("File path ({filePath}) must not be null or empty.", filePath);
                    throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
                }
                using FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamReader reader = new StreamReader(fileStream);

                fileStream.Position = 0;
                var content = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(content))
                {
                    Log.Warning("File is empty. Returning empty data.");

                    return Activator.CreateInstance<T>() ?? throw new InvalidOperationException("Failed to create default instance of type.");
                }

                var value = JsonSerializer.Deserialize<T>(content);
                if (value is null)
                {
                    throw new InvalidDataException("Failed to deserialize data from file: content is not in valid format.");
                }

                Log.Information($"Successfully read data from file.");
                return value;
            }
            catch (JsonException ex)
            {
                Log.Error(ex, "File content is not valid JSON.");
                throw new InvalidDataException("File content is not valid JSON.", ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to read data from file at {filePath}",filePath);
                throw;
            }
        }
    }
}
