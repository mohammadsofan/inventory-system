using InventoryApp.Models;
using System;
using System.Collections.Generic;
using InventoryApp.Dtos;

namespace InventoryApp.Interfaces
{
    internal interface IProductService
    {
        GetProductsResultDto GetAllProducts(Func<Product, bool>? filter);
        GetProductResultDto GetProductByFilter(Func<Product, bool> filter);
        ProductOperationResultDto Create(Product product);
        DeleteResultDto DeleteProduct(long id);
        ProductOperationResultDto UpdateProduct(long id, Product product);
        long GetNextId();

    }
}
