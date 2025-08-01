using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Models
{
    internal class Product
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } =string.Empty;
        public double Price { get; set; }
        public double Discount { get; set; }
        public double FinalPrice => Math.Round(Price - Price * Discount,2); 
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"""
                Id:{Id},
                Name:{Name},
                Description:{Description},
                Price:{Price},
                Discount:{Discount},
                FinalPrice:{FinalPrice},
                Quantity:{Quantity},
                CreatedAt:{CreatedAt}
                """;
        }
    }
}
