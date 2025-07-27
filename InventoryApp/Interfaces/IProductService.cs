using InventoryApp.Models;
using System;
using System.Collections.Generic;
using InventoryApp.Dtos;

namespace InventoryApp.Interfaces
{
    internal interface IProductService
    {
        GetProductsResult GetProducts(Func<Product, bool>? filter);
        GetProductResult GetProduct(Func<Product, bool> filter);
        Result Create(Product product);
        DeleteResult Delete(long id);
        Result Update(long id, Product product);
        long GetNextId();

    }
}
