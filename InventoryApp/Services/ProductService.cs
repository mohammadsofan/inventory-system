using InventoryApp.Interfaces;
using InventoryApp.Models;
using InventoryApp.Validators;
using InventoryApp.Dtos;
using Serilog;

namespace InventoryApp.Services
{
    internal class ProductService : IProductService
    {
        private readonly IFileService<List<Product>> fileService;
        private readonly ProductValidator validator;
        private readonly string FilePath;
        public ProductService(string path)
        {
            fileService = new FileService<List<Product>>();
            FilePath = path;
            validator = new ProductValidator();
        }
        public ProductOperationResultDto CreateProduct(Product product)
        {
            try
            {
                Log.Information("Attempting to create a new product.");
                var validationResult = validator.Validate(product);
                if (validationResult.IsValid)
                {
                    var products = fileService.ReadFromFile(FilePath);
                    products.Add(product);
                    fileService.WriteToFile(products, FilePath);
                    Log.Information("Product with ID {product.Id} created successfully.", product.Id);
                    return new ProductOperationResultDto()
                    {
                        Message = "Product Created Successfully!",
                        Success = true,
                    };
                }
                else
                {
                    Log.Warning("Validation failed while creating product. Errors: {Errors}", string.Join(", ", validationResult.Errors.Select(e => $"{e.Field}: {e.Message}")));
                    return new ProductOperationResultDto()
                    {
                        Message = "Fail to create product due to validation errors.",
                        Success = false,
                        ValidationResult = validationResult
                    };

                }
            }
            catch (Exception ex) when (ex is InvalidDataException)
            {
                Log.Error(ex, "InvalidDataException occurred during Create.");
                return new ProductOperationResultDto()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred during Create.");
                return new ProductOperationResultDto()
                {
                    Message = "An unexpected error occurred.",
                    Success = false,
                };
            }
        }

        public DeleteResultDto DeleteProduct(long id)
        {
            Log.Information("Attempting to delete product with ID {id}.",id);
            try
            {
                var products = fileService.ReadFromFile(FilePath);
                var product = products.FirstOrDefault(p => p.Id == id);
                if (product is null)
                {
                    Log.Warning("Delete failed: product with ID {id} not found.",id);
                    return new DeleteResultDto()
                    {
                        Message = $"Product with ID {id} not found.",
                        Success = false,
                    };
                }
                else
                {
                    products.Remove(product);
                    fileService.WriteToFile(products, FilePath);
                    Log.Information("Product with ID {id} deleted successfully.",id);
                    return new DeleteResultDto()
                    {
                        Message = "Product deleted Successfully!",
                        Success = true,
                    };
                }

            }
            catch (Exception ex) when (ex is InvalidDataException)
            {
                Log.Error(ex, "InvalidDataException occurred during Delete.");
                return new DeleteResultDto()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred during Delete.");

                return new DeleteResultDto()
                {
                    Message = "An unexpected error occurred.",
                    Success = false,
                };
            }
        }

        public GetProductResultDto GetProductByFilter(Func<Product, bool> filter)
        {
            try
            {
                Log.Information("Retrieving product by filter");

                var products = GetAllProducts(filter).Products;
                Product? product = products?.FirstOrDefault();
                if (product is not null)
                {
                    Log.Information("Retrieving product by filter successed,product id = {Id}",product.Id);

                    return new GetProductResultDto()
                    {
                        Product = product,
                    };
                }
                else
                {
                    Log.Warning("product not found");

                    return new GetProductResultDto()
                    {
                        Message = "Product Not Found.",
                    };
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred during Retrieving product.");

                return new GetProductResultDto()
                {
                    Message = "An unexpected error occurred."
                };
            }
        }

        public GetProductsResultDto GetAllProducts(Func<Product, bool>? filter)
        {
            Log.Information("Retrieving product list...");
            try
            {
                var products = fileService.ReadFromFile(FilePath);
                if(filter is not null)
                {
                    products = products.Where(filter).ToList();
                }
                Log.Information("Retrieved {Count} product(s).",products.Count);
                return new GetProductsResultDto()
                {
                    Products = products,
                };
            }
            catch(Exception ex) when (ex is InvalidDataException)
            {
                Log.Error(ex, "InvalidDataException occurred during Retrieving product list.");
                return new GetProductsResultDto() { 
                    Products = new List<Product>(),
                    Message = "Something went wrong, could be Invalid file format.",

                };
            }
            
            catch(Exception ex )
            {
                Log.Error(ex, "Unexpected error occurred during Retrieving product list.");

                return new GetProductsResultDto()
                {
                    Products = new List<Product>(),
                    Message = "An unexpected error occurred."
                };
            }
        }

        public ProductOperationResultDto UpdateProduct(long id, Product product)
        {
            try
            {
                Log.Information("Attempting to update product with ID {id}.",id);
                var products = fileService.ReadFromFile(FilePath);
                var existingProduct = products.FirstOrDefault(p => p.Id == id);
                if(existingProduct is null) {
                    Log.Warning("Update failed: product with ID {id} not found.",id);
                    return new ProductOperationResultDto()
                    {
                        Message = $"Product with ID {id} not found.",
                        Success = false,
                    };
                }
                var validationResult = validator.Validate(product);
                if (validationResult.IsValid)
                {
                    var index = products.IndexOf(existingProduct);
                    product.Id = existingProduct.Id;
                    product.CreatedAt = existingProduct.CreatedAt;
                    products[index] = product;
                    fileService.WriteToFile(products, FilePath);
                    Log.Information("Product with ID {id} updated successfully.",id);
                    return new ProductOperationResultDto()
                    {
                        Message = "Product updated Successfully!",
                        Success = true,
                    };
                }
                else
                {
                    Log.Warning("Validation failed while updating product with ID {id}. Errors: {Errors}",id, string.Join(", ", validationResult.Errors.Select(e => $"{e.Field}: {e.Message}")));

                    return new ProductOperationResultDto()
                    {
                        Message = "Product update failed due to validation errors.",
                        Success = false,
                        ValidationResult = validationResult
                    };
                }
            }
            catch (Exception ex) when (ex is InvalidDataException)
            {
                Log.Error(ex, "InvalidDataException occurred during Update.");
                return new ProductOperationResultDto()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error occurred during Update.");
                return new ProductOperationResultDto()
                {
                    Message = "An unexpected error occurred.",
                    Success = false,
                };
            }
        }
        public long GetNextProductId()
        {
            try
            {
                Log.Information( "Trying to get next id");

                var products = fileService.ReadFromFile(FilePath);
                if(products.Count == 0)
                {
                    Log.Information("returned id with value {id}",1);

                    return 1;
                }
                var currentMaxId = products.Max(x => x.Id);
                Log.Information("returned id with value {id}", currentMaxId+1);
                return currentMaxId + 1;
            }
            catch(Exception ex)
            {
                Log.Error(ex,"Error occured while trying to get next id, value returend {id}",-1);

                return -1;
            }
        }
    }
}
