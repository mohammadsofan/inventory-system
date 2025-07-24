using InventoryApp.Models;
using InventoryApp.Services;

namespace InventoryApp
{
    internal class Program
    {
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
        public static Product ReadProductInfo()
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
            product.Id=Guid.NewGuid();
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
        public static Guid GetProductId()
        {
            Console.Write("Enter product ID :");
            var IsValidId = Guid.TryParse(Console.ReadLine(), out var id);
            while(!IsValidId)
            {
                Console.Write("Invalid Guid Format, please enter a valid id with Guid format :");
                IsValidId = Guid.TryParse(Console.ReadLine(),out id);
            }
            return id;
        }
        static void Main(string[] args)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "products.json");
            ProductService productService = new ProductService(path);
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
                    var result = productService.Create(ReadProductInfo());
                    Console.WriteLine(result.Message);
                    if (result.ValidationResult != null && result.ValidationResult.IsValid == false)
                    {
                        foreach(var error in result.ValidationResult.Errors)
                            {
                                Console.WriteLine($"Error: FieldName[{error.Field}], {error.Message} ");
                            }
                    }
                    goto displayOptionsList;
                           
                }
                case 2:
                {
                    var result = productService.GetProducts(null);
                    if (result.Products == null)
                    {
                        Console.WriteLine(result.Message);
                    }
                    else
                    {
                        PrintProducts(result.Products);
                    }
                    goto displayOptionsList;
                }
                case 3:
                {
                    Guid id = GetProductId();
                    var result = productService.GetProduct(p => p.Id == id);
                    if(result.Product is null)
                    {
                        Console.WriteLine(result.Message);
                    }
                    else
                    {
                        PrintProduct(result.Product);
                    }
                    goto displayOptionsList;
                }
                case 4:
                {
                    Guid id = GetProductId();
                    var product = productService.GetProduct(p => p.Id == id).Product;
                    if (product is null)
                    {
                        Console.WriteLine($"Product with ID {id} not found.");
                    }
                    else
                    {
                        var result = productService.Update(id, ReadProductInfo());
                        Console.WriteLine(result.Message);
                        if (result.ValidationResult != null && result.ValidationResult.IsValid == false)
                        {
                            foreach (var error in result.ValidationResult.Errors)
                            {
                                Console.WriteLine($"Error: FieldName[{error.Field}], {error.Message} ");
                            }
                        }
                    }
                    goto displayOptionsList;
                }
                case 5:
                {
                    Guid id = GetProductId();
                    var result = productService.Delete(id);
                    Console.WriteLine(result.Message);
                    goto displayOptionsList;
                }
                case 6:
                {
                    return;
                }
            }
        }
    }
}
