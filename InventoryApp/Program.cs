using InventoryApp.Dtos;
using InventoryApp.Interfaces;
using InventoryApp.Models;
using InventoryApp.Services;
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
        public static void DisplayOptionsList()
        {
            Console.WriteLine("""
                    ==================== Inventory System ======================

                    Please choose one of the following options:

                    1. Create a new product
                    2. View all products
                    3. View a single product by ID
                    4. Update a product
                    5. Delete a product
                    6. Exit the application

                    ============================================================
                    """);
        }
        public static Product ReadProductInfo(long id)
        {
            
            Product product = new Product();
            Console.Write("Product Name : ");
            product.Name = Console.ReadLine()!;
            Console.Write("Product Description : ");
            product.Description = Console.ReadLine()!;
            Console.Write("Product Price : ");
            var isValidValue = double.TryParse(Console.ReadLine(), out var price);
            while (!isValidValue)
            {
                Console.WriteLine("please enter numeric value.");
                isValidValue = double.TryParse(Console.ReadLine(), out price);
            }
            product.Price = price;

            double discount;
            bool IsValidRange;
            do
            {
                IsValidRange = true;
                Console.Write("Product Discount : ");
                isValidValue = double.TryParse(Console.ReadLine(), out discount);
                if (!isValidValue)
                {
                    Console.WriteLine("Please enter numeric value.");
                    continue;
                }
                if (discount < 0 || discount > 1)
                {
                    Console.WriteLine("Discount value must be between 0.00 and 1.00");
                    IsValidRange = false;
                }
            } while (!isValidValue || !IsValidRange);
            product.Discount = discount;

            Console.Write("Product Quantity : ");
            isValidValue = int.TryParse(Console.ReadLine(), out var quantity);
            while (!isValidValue)
            {
                Console.WriteLine("please enter numeric value.");
                isValidValue = int.TryParse(Console.ReadLine(), out quantity);
            }
            product.Quantity = quantity;
            
            product.Id = id;
            product.CreatedAt = DateTime.Now;
            return product;
        }
        public static void PrintProduct(Product product)
        {
            Console.WriteLine("========ProductInfo=========");
            Console.WriteLine(product);
            Console.WriteLine("============================");
        }
        public static void PrintProducts(IList<Product>? products)
        {
            if (products is null) return;
            if(!products.Any())
            {
                Console.WriteLine("There are no products.");
                return;
            }
            foreach (var product in products)
            {
                PrintProduct(product);
            }
            Console.WriteLine($"Total Products : {products.Count}");
        }
        public static long GetProductId()
        {
            Console.Write("Enter product ID :");
            var IsValidId = long.TryParse(Console.ReadLine(), out var id);
            while(!IsValidId)
            {
                Console.Write("Invalid ID Format, please enter a Numeric ID:");
                IsValidId = long.TryParse(Console.ReadLine(),out id);
            }
            return id;
        }
        static void Main(string[] args)
        {


            Logger.LogInformation("Application started");

            var path = Path.Combine(AppContext.BaseDirectory, "Data", "products.json");
            IProductService productService = new ProductService(path);
            displayOptionsList:
            DisplayOptionsList();
            var isNumericinput = int.TryParse(Console.ReadLine(),out var option);
            while(!isNumericinput || option < 1 || option > 6)
            {
                Console.WriteLine("Invalid option, please enter a valid option.");
                isNumericinput = int.TryParse(Console.ReadLine(), out option);
            }
            switch(option)
            {
                case 1:
                {
                    var product = ReadProductInfo(productService.GetNextProductId());
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
                case 2:
                {
                    var result = productService.GetAllProducts(null);
                    Logger.LogInformation($"Getting all products");
                    if (result.Products == null)
                    {
                        Logger.LogInformation($"Getting all products, result = {result.Message}");
                        Console.WriteLine(result.Message);
                    }
                    else
                    {
                        Logger.LogInformation($"Getting all products, result = printed");
                        PrintProducts(result.Products);
                    }
                    goto displayOptionsList;
                }
                case 3:
                {
                    long id = GetProductId();
                    Logger.LogInformation($"Getting product with ID = {id}");
                    var result = productService.GetProductByFilter(p => p.Id == id);
                    if(result.Product is null)
                    {
                        Console.WriteLine(result.Message);
                        Logger.LogInformation($"Getting product with ID = {id}, result = {result.Message}");
                    }
                    else
                    {
                        PrintProduct(result.Product);
                        Logger.LogInformation($"Getting product with ID = {id}, result = printed");
                    }
                        goto displayOptionsList;
                }
                case 4:
                {
                    long id = GetProductId();
                    var product = productService.GetProductByFilter(p => p.Id == id).Product;
                    Logger.LogInformation($"Trying to update product with ID = {id}");
                    if (product is null)
                    {
                        Logger.LogWarning($"Update failed, product with ID = {id} not found");
                        Console.WriteLine($"Product with ID {id} not found.");
                    }
                    else
                    {
                        var result = productService.UpdateProduct(id, ReadProductInfo(id));
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
                case 5:
                {
                    long id = GetProductId();
                    var result = productService.DeleteProduct(id);
                    Console.WriteLine(result.Message);
                    Logger.LogInformation($"deleting product with ID = {id} , Result = {result.Message}");
                    goto displayOptionsList;
                }
                case 6:
                {
                    Logger.LogInformation("Application Closed");
                    return;
                }
            }
        }
    }
}
