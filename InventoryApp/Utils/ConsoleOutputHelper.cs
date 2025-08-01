using InventoryApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Utils
{
    internal class ConsoleOutputHelper
    {
        public void PrintProduct(Product product)
        {
            Console.WriteLine("========ProductInfo=========");
            Console.WriteLine(product);
            Console.WriteLine("============================");
        }
        public void PrintProducts(IList<Product>? products)
        {
            if (products is null) return;
            if (!products.Any())
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
    }
}
