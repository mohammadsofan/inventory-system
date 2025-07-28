using InventoryApp.Enums;
using InventoryApp.Interfaces;
using InventoryApp.Models;
using InventoryApp.Services;
using InventoryApp.Utils;
using Microsoft.Extensions.Logging;
using Serilog;

namespace InventoryApp
{
    internal class Program
    {
        public static Microsoft.Extensions.Logging.ILogger Logger;
        static Program()
        {
            var sessionId = Guid.NewGuid().ToString();
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("SessionId",sessionId)
                .MinimumLevel.Information()
                .WriteTo.File("Logs/log.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} | SessionId={SessionId}{NewLine}{Exception}")
                .CreateLogger();
            var loggerFactory = LoggerFactory.Create(config =>
            {
                config.AddSerilog();
            });
            Logger = loggerFactory.CreateLogger<Program>();
        }

        static void Main(string[] args)
        {
            Logger.LogInformation("Application started");

            var appMenu = new InventoryAppMenu();
            var consoleInputHelper = new ConsoleInputHelper();
            var consoleOutputHelper = new ConsoleOutputHelper();
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "products.json");
            IProductService productService = new ProductService(path);

            displayOptionsList:
            appMenu.DisplayOptionsList();
            var isNumericinput = int.TryParse(Console.ReadLine(),out var option);
            while(!isNumericinput || option < 1 || option > 6)
            {
                Console.WriteLine("Invalid option, please enter a valid option.");
                isNumericinput = int.TryParse(Console.ReadLine(), out option);
            }
            ConsoleMenuOperation operation = (ConsoleMenuOperation)option;
            switch(operation)
            {
                case ConsoleMenuOperation.Create:
                {
                    long id = productService.GetNextProductId();
                    var product = consoleInputHelper.ReadProductInfo(id,3,3);
                    var result = productService.CreateProduct(product);
                    Logger.LogInformation($"Creating new product ID = {product.Id}, Name = {product.Name},..., result = {result.Message}");
                    Console.WriteLine(result.Message);
                    if (result.ValidationResult != null && result.ValidationResult.IsValid == false)
                    {
                        foreach(var error in result.ValidationResult.Errors)
                            {
                                Logger.LogInformation($"Creating new product failed, Error: FieldName[{error.Field}], {error.Message}");
                                Console.WriteLine($"Error: FieldName[{error.Field}], {error.Message} ");
                            }
                    }
                    goto displayOptionsList;
                           
                }
                case ConsoleMenuOperation.GetAllProducts:
                {
                    var result = productService.GetAllProducts(null);
                    Logger.LogInformation($"Getting all products");
                    if (result.Products.Count == 0 && result.Message.Length !=0)
                    {
                        Logger.LogInformation($"Getting all products, result = {result.Message}");
                        Console.WriteLine(result.Message);
                    }
                    else
                    {
                        Logger.LogInformation($"Getting all products, result = printed");
                        consoleOutputHelper.PrintProducts(result.Products);
                    }
                    goto displayOptionsList;
                }
                case ConsoleMenuOperation.GetOneProductById:
                {
                    long id = consoleInputHelper.ReadProductId();
                    Logger.LogInformation($"Getting product with ID = {id}");
                    var result = productService.GetProductByFilter(p => p.Id == id);
                    if(result.Product is null)
                    {
                        Console.WriteLine(result.Message);
                        Logger.LogInformation($"Getting product with ID = {id}, result = {result.Message}");
                    }
                    else
                    {
                        consoleOutputHelper.PrintProduct(result.Product);
                        Logger.LogInformation($"Getting product with ID = {id}, result = printed");
                    }
                        goto displayOptionsList;
                }
                case ConsoleMenuOperation.UpdateProduct:
                {
                    long id = consoleInputHelper.ReadProductId();
                    var product = productService.GetProductByFilter(p => p.Id == id).Product;
                    Logger.LogInformation($"Trying to update product with ID = {id}");
                    if (product is null)
                    {
                        Logger.LogWarning($"Update failed, product with ID = {id} not found");
                        Console.WriteLine($"Product with ID {id} not found.");
                    }
                    else
                    {
                        var result = productService.UpdateProduct(id, consoleInputHelper.ReadProductInfo(id,3,3));
                        Console.WriteLine(result.Message);
                        Logger.LogInformation($"Update result, product with ID = {id}, Result = {result.Message}");
                        if (result.ValidationResult != null && result.ValidationResult.IsValid == false)
                        {
                            foreach (var error in result.ValidationResult.Errors)
                            {
                                Logger.LogWarning($"Update Error, product with ID = {id}, Error: FieldName[{error.Field}], {error.Message}");
                                Console.WriteLine($"Error: FieldName[{error.Field}], {error.Message} ");
                            }
                        }
                    }
                    goto displayOptionsList;
                }
                case ConsoleMenuOperation.DeleteProduct:
                {
                    long id = consoleInputHelper.ReadProductId();
                    var result = productService.DeleteProduct(id);
                    Console.WriteLine(result.Message);
                    Logger.LogInformation($"deleting product with ID = {id} , Result = {result.Message}");
                    goto displayOptionsList;
                }
                case ConsoleMenuOperation.Exit:
                {
                    Logger.LogInformation("Application Closed");
                    return;
                }
            }
        }
    }
}
