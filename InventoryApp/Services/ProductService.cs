using InventoryApp.Interfaces;
using InventoryApp.Models;
using InventoryApp.Validators;
using InventoryApp.Dtos;
using Microsoft.Extensions.Logging;
using Serilog;

namespace InventoryApp.Services
{
    internal class ProductService : IProductService
    {
        private readonly IFileService fileService;
        private readonly ProductValidator validator;
        private readonly Microsoft.Extensions.Logging.ILogger Logger;
        public ProductService(string path)
        {
            fileService = new FileService(path);
            validator = new ProductValidator();

            var loggerFactory = LoggerFactory.Create(config =>
            {
                config.AddSerilog();
            });
            Logger = loggerFactory.CreateLogger<Program>();
        }
        public Result Create(Product product)
        {
            try
            {
                Logger.LogInformation("Attempting to create a new product.");
                var validatorResult = validator.Validate(product);
                if (validatorResult.IsValid)
                {
                    var products = fileService.ReadFromFile();
                    products.Add(product);
                    fileService.WriteToFile(products);
                    Logger.LogInformation($"Product with ID {product.Id} created successfully.");
                    return new Result()
                    {
                        Message = "Product Created Successfully!",
                        Success = true,
                    };
                }
                else
                {
                    Logger.LogWarning($"Validation failed while creating product. Errors: {string.Join(", ", validatorResult.Errors.Select(e => $"{e.Field}: {e.Message}"))}");
                    return new Result()
                    {
                        Message = "Fail to create product due to validation errors.",
                        Success = false,
                        ValidationResult = validatorResult
                    };

                }
            }
            catch (Exception ex) when (ex is InvalidDataException)
            {
                Logger.LogError(ex, "InvalidDataException occurred during Create.");
                return new Result()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred during Create.");
                return new Result()
                {
                    Message = "An unexpected error occurred.",
                    Success = false,
                };
            }
        }

        public DeleteResult Delete(long id)
        {
            Logger.LogInformation($"Attempting to delete product with ID {id}.");
            try
            {
                var products = fileService.ReadFromFile();
                var product = products.FirstOrDefault(p => p.Id == id);
                if (product is null)
                {
                    Logger.LogWarning($"Delete failed: product with ID {id} not found.");
                    return new DeleteResult()
                    {
                        Message = $"Product with ID {id} not found.",
                        Success = false,
                    };
                }
                else
                {
                    products.Remove(product);
                    fileService.WriteToFile(products);
                    Logger.LogInformation($"Product with ID {id} deleted successfully.");
                    return new DeleteResult()
                    {
                        Message = "Product deleted Successfully!",
                        Success = true,
                    };
                }

            }
            catch (Exception ex) when (ex is InvalidDataException)
            {
                Logger.LogError(ex, "InvalidDataException occurred during Delete.");
                return new DeleteResult()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred during Delete.");

                return new DeleteResult()
                {
                    Message = "An unexpected error occurred.",
                    Success = false,
                };
            }
        }

        public GetProductResult GetProduct(Func<Product, bool> filter)
        {
            try
            {
                Logger.LogInformation("Retrieving product by filter }");

                var products = GetProducts(filter).Products;
                Product? product = null;
                if (products is not null)
                {
                    product = products.FirstOrDefault();
                }
                else
                {
                    Logger.LogInformation("Retrieving product by filter failed }");

                    return new GetProductResult()
                    {
                        Message = "Failed to retrieve products from file."
                    };
                }
                if (product is not null)
                {
                    Logger.LogInformation($"Retrieving product by filter successed,product id = {product.Id}");

                    return new GetProductResult()
                    {
                        Product = product,
                    };
                }
                else
                {
                    Logger.LogWarning("product not found");

                    return new GetProductResult()
                    {
                        Message = "Product Not Found.",
                    };
                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred during Retrieving product.");

                return new GetProductResult()
                {
                    Message = "An unexpected error occurred."
                };
            }
        }

        public GetProductsResult GetProducts(Func<Product, bool>? filter)
        {
            Logger.LogInformation("Retrieving product list...");
            try
            {
                var products = fileService.ReadFromFile();
                if(filter is not null)
                {
                    products = products.Where(filter).ToList();
                }
                Logger.LogInformation($"Retrieved {products.Count} product(s).");
                return new GetProductsResult()
                {
                    Products = products,
                    Message = null
                };
            }
            catch(Exception ex) when (ex is InvalidDataException)
            {
                Logger.LogError(ex, "InvalidDataException occurred during Retrieving product list.");
                return new GetProductsResult() { 
                    Products = null,
                    Message = "Something went wrong, could be Invalid file format.",

                };
            }
            
            catch(Exception ex )
            {
                Logger.LogError(ex, "Unexpected error occurred during Retrieving product list.");

                return new GetProductsResult()
                {
                    Products = null,
                    Message = "An unexpected error occurred."
                };
            }
        }

        public Result Update(long id, Product product)
        {
            try
            {
                Logger.LogInformation($"Attempting to update product with ID {id}.");
                var products = fileService.ReadFromFile();
                var oldProduct = products.FirstOrDefault(p => p.Id == id);
                if(oldProduct is null) {
                    Logger.LogWarning($"Update failed: product with ID {id} not found.");
                    return new Result()
                    {
                        Message = $"Product with ID {id} not found.",
                        Success = false,
                    };
                }
                var validatorResult = validator.Validate(product);
                if (validatorResult.IsValid)
                {
                    var index = products.IndexOf(oldProduct);
                    product.Id = oldProduct.Id;
                    product.CreatedAt = oldProduct.CreatedAt;
                    products[index] = product;
                    fileService.WriteToFile(products);
                    Logger.LogInformation($"Product with ID {id} updated successfully.");
                    return new Result()
                    {
                        Message = "Product updated Successfully!",
                        Success = true,
                    };
                }
                else
                {
                    Logger.LogWarning($"Validation failed while updating product with ID {id}. Errors: {string.Join(", ", validatorResult.Errors.Select(e => $"{e.Field}: {e.Message}"))}");

                    return new Result()
                    {
                        Message = "Product update failed due to validation errors.",
                        Success = false,
                        ValidationResult = validatorResult
                    };
                }
            }
            catch (Exception ex) when (ex is InvalidDataException)
            {
                Logger.LogError(ex, "InvalidDataException occurred during Update.");
                return new Result()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred during Update.");
                return new Result()
                {
                    Message = "An unexpected error occurred.",
                    Success = false,
                };
            }
        }
        public long GetNextId()
        {
            try
            {
                var products = fileService.ReadFromFile();
                if(products.Count == 0)
                {
                    return 1;
                }
                var currentMaxId = products.Max(x => x.Id);
                return currentMaxId + 1;
            }
            catch(Exception)
            {
                return -1;
            }
        }
    }
}
