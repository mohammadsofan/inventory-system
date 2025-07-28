using InventoryApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Utils
{
    internal class ConsoleInputHelper
    {
        public Product ReadProductInfo(long id,int nameMinLength=0,int descriptionMinLength=0)
        {
            var product = new Product
            {
                Name = ReadString("Product Name: ", minLength: nameMinLength),
                Description = ReadString("Product Description: ", minLength: descriptionMinLength),
                Price = ReadDouble("Product Price: "),
                Discount = ReadDiscount("Product Discount: "),
                Quantity = ReadInt("Product Quantity: "),
                Id = id,
                CreatedAt = DateTime.Now
            };
            return product;
        }

        private string ReadString(string prompt, int minLength = 0)
        {
            string? input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine() ?? string.Empty;
                if (input.Length < minLength)
                {
                    Console.WriteLine($"Input must be at least {minLength} characters.");
                }
            } while (input.Length < minLength);

            return input;
        }

        private double ReadDouble(string prompt)
        {
            double value;
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out value))
                {
                    return value;
                }
                Console.WriteLine("Please enter a numeric value.");
            }
        }

        private double ReadDiscount(string prompt)
        {
            double discount;
            while (true)
            {
                Console.Write(prompt);
                if (!double.TryParse(Console.ReadLine(), out discount))
                {
                    Console.WriteLine("Please enter a numeric value.");
                    continue;
                }
                if (discount < 0 || discount > 1)
                {
                    Console.WriteLine("Discount value must be between 0.00 and 1.00");
                    continue;
                }
                return discount;
            }
        }

        private int ReadInt(string prompt)
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out value))
                {
                    return value;
                }
                Console.WriteLine("Please enter a numeric value.");
            }
        }
        public long ReadProductId()
        {
            Console.Write("Enter product ID :");
            var IsValidId = long.TryParse(Console.ReadLine(), out var id);
            while (!IsValidId)
            {
                Console.Write("Invalid ID Format, please enter a Numeric ID:");
                IsValidId = long.TryParse(Console.ReadLine(), out id);
            }
            return id;
        }
    }
}
