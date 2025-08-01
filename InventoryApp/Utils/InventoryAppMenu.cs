using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Utils
{
    internal class InventoryAppMenu
    {
        public void DisplayOptionsList()
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
    }
}
