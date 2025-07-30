using InventoryApp.Enums;
using InventoryApp.Interfaces;
using InventoryApp.Services;
using InventoryApp.Utils;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace InventoryApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configurations = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configurations)
                .CreateLogger();
            var logPath = Path.Combine(AppContext.BaseDirectory, "Logs");
            Directory.CreateDirectory(logPath);
            try
            {
                Log.Information("Application started");
                var appMenu = new InventoryAppMenu();
                var consoleInputHelper = new ConsoleInputHelper();
                var consoleOutputHelper = new ConsoleOutputHelper();
                var path = Path.Combine(AppContext.BaseDirectory, "Data", "products.json");
                IProductService productService = new ProductService(path);

            displayOptionsList:
                appMenu.DisplayOptionsList();
                var isNumericinput = int.TryParse(Console.ReadLine(), out var option);
                while (!isNumericinput || option < 1 || option > 6)
                {
                    Console.WriteLine("Invalid option, please enter a valid option.");
                    isNumericinput = int.TryParse(Console.ReadLine(), out option);
                }
                ConsoleMenuOperation operation = (ConsoleMenuOperation)option;
                switch (operation)
                {
                    case ConsoleMenuOperation.Create:
                        {
                            long id = productService.GetNextProductId();
                            var product = consoleInputHelper.ReadProductInfo(id, 3, 3);
                            var result = productService.CreateProduct(product);
                            Log.Information("Creating new product ID = {Id}, Name = {Name},..., result = {Message}", product.Id,product.Name,result.Message);
                            Console.WriteLine(result.Message);
                            if (result.ValidationResult != null && result.ValidationResult.IsValid == false)
                            {
                                foreach (var error in result.ValidationResult.Errors)
                                {
                                    Log.Information("Creating new product failed due to validation error, Error: FieldName[{Field}], {Message}",error.Field,error.Message);
                                    Console.WriteLine($"Error: FieldName[{error.Field}], {error.Message} ");
                                }
                            }
                            goto displayOptionsList;

                        }
                    case ConsoleMenuOperation.GetAllProducts:
                        {
                            var result = productService.GetAllProducts(null);
                            Log.Information($"Getting all products");
                            if (result.Products.Count == 0 && result.Message.Length != 0)
                            {
                                Log.Information("Getting all products, result = {Message}",result.Message);
                                Console.WriteLine(result.Message);
                            }
                            else
                            {
                                Log.Information($"Getting all products, result = printed");
                                consoleOutputHelper.PrintProducts(result.Products);
                            }
                            goto displayOptionsList;
                        }
                    case ConsoleMenuOperation.GetOneProductById:
                        {
                            long id = consoleInputHelper.ReadProductId();
                            Log.Information("Getting product with ID = {id}",id);
                            var result = productService.GetProductByFilter(p => p.Id == id);
                            if (result.Product is null)
                            {
                                Console.WriteLine(result.Message);
                                Log.Information("Getting product with ID = {id}, result = {Message}",id,result.Message);
                            }
                            else
                            {
                                consoleOutputHelper.PrintProduct(result.Product);
                                Log.Information("Getting product with ID = {id}, result = printed",id);
                            }
                            goto displayOptionsList;
                        }
                    case ConsoleMenuOperation.UpdateProduct:
                        {
                            long id = consoleInputHelper.ReadProductId();
                            var product = productService.GetProductByFilter(p => p.Id == id).Product;
                            Log.Information("Trying to update product with ID = {id}",id);
                            if (product is null)
                            {
                                Log.Warning("Update failed, product with ID = {id} not found",id);
                                Console.WriteLine($"Product with ID {id} not found.");
                            }
                            else
                            {
                                var result = productService.UpdateProduct(id, consoleInputHelper.ReadProductInfo(id, 3, 3));
                                Console.WriteLine(result.Message);
                                Log.Information("Update result, product with ID = {id}, Result = {Message}",id,result.Message);
                                if (result.ValidationResult != null && result.ValidationResult.IsValid == false)
                                {
                                    foreach (var error in result.ValidationResult.Errors)
                                    {
                                        Log.Warning("Update Error, product with ID = {id}, Error: FieldName[{Field}], {Message}",id,error.Field,error.Message);
                                        Console.WriteLine($"Error: FieldName[{error.Field}], {error.Message} ");
                                    }
                                }
                            }
                            goto displayOptionsList;
                        }
                    case ConsoleMenuOperation.DeleteProduct:
                        {
                            long id = consoleInputHelper.ReadProductId();
                            var confirm = consoleInputHelper.ConfirmDeletion();
                            if (confirm == false)
                            {
                                goto displayOptionsList;
                            }
                            var result = productService.DeleteProduct(id);
                            Console.WriteLine(result.Message);
                            Log.Information("deleting product with ID = {id} , Result = {Message}",id,result.Message);
                            goto displayOptionsList;
                        }
                    case ConsoleMenuOperation.Exit:
                        {
                            Log.Information("Application Closed");
                            return;
                        }
                }
            }catch(Exception ex)
            {
                Log.Error(ex, "Error occured, program terminated");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

    }
}
