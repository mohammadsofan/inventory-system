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
        public ProductOperationResultDto CreateProduct(Product product)
        {
            try
            {
                Logger.LogInformation("Attempting to create a new product.");
                var validationResult = validator.Validate(product);
                if (validationResult.IsValid)
                {
                    var products = fileService.ReadFromFile();
                    products.Add(product);
                    fileService.WriteToFile(products);
                    Logger.LogInformation($"Product with ID {product.Id} created successfully.");
                    return new ProductOperationResultDto()
                    {
                        Message = "Product Created Successfully!",
                        Success = true,
                    };
                }
                else
                {
                    Logger.LogWarning($"Validation failed while creating product. Errors: {string.Join(", ", validationResult.Errors.Select(e => $"{e.Field}: {e.Message}"))}");
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
                Logger.LogError(ex, "InvalidDataException occurred during Create.");
                return new ProductOperationResultDto()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred during Create.");
                return new ProductOperationResultDto()
                {
                    Message = "An unexpected error occurred.",
                    Success = false,
                };
            }
        }

        public DeleteResultDto DeleteProduct(long id)
        {
            Logger.LogInformation($"Attempting to delete product with ID {id}.");
            try
            {
                var products = fileService.ReadFromFile();
                var product = products.FirstOrDefault(p => p.Id == id);
                if (product is null)
                {
                    Logger.LogWarning($"Delete failed: product with ID {id} not found.");
                    return new DeleteResultDto()
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
                    return new DeleteResultDto()
                    {
                        Message = "Product deleted Successfully!",
                        Success = true,
                    };
                }

            }
            catch (Exception ex) when (ex is InvalidDataException)
            {
                Logger.LogError(ex, "InvalidDataException occurred during Delete.");
                return new DeleteResultDto()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred during Delete.");

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
                Logger.LogInformation("Retrieving product by filter }");

                var products = GetAllProducts(filter).Products;
                Product? product = null;
                if (products is not null)
                {
                    product = products.FirstOrDefault();
                }
                else
                {
                    Logger.LogInformation("Retrieving product by filter failed }");

                    return new GetProductResultDto()
                    {
                        Message = "Failed to retrieve products from file."
                    };
                }
                if (product is not null)
                {
                    Logger.LogInformation($"Retrieving product by filter successed,product id = {product.Id}");

                    return new GetProductResultDto()
                    {
                        Product = product,
                    };
                }
                else
                {
                    Logger.LogWarning("product not found");

                    return new GetProductResultDto()
                    {
                        Message = "Product Not Found.",
                    };
                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred during Retrieving product.");

                return new GetProductResultDto()
                {
                    Message = "An unexpected error occurred."
                };
            }
        }

        public GetProductsResultDto GetAllProducts(Func<Product, bool>? filter)
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
                return new GetProductsResultDto()
                {
                    Products = products,
                    Message = null
                };
            }
            catch(Exception ex) when (ex is InvalidDataException)
            {
                Logger.LogError(ex, "InvalidDataException occurred during Retrieving product list.");
                return new GetProductsResultDto() { 
                    Products = null,
                    Message = "Something went wrong, could be Invalid file format.",

                };
            }
            
            catch(Exception ex )
            {
                Logger.LogError(ex, "Unexpected error occurred during Retrieving product list.");

                return new GetProductsResultDto()
                {
                    Products = null,
                    Message = "An unexpected error occurred."
                };
            }
        }

        public ProductOperationResultDto UpdateProduct(long id, Product product)
        {
            try
            {
                Logger.LogInformation($"Attempting to update product with ID {id}.");
                var products = fileService.ReadFromFile();
                var existingProduct = products.FirstOrDefault(p => p.Id == id);
                if(existingProduct is null) {
                    Logger.LogWarning($"Update failed: product with ID {id} not found.");
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
                    fileService.WriteToFile(products);
                    Logger.LogInformation($"Product with ID {id} updated successfully.");
                    return new ProductOperationResultDto()
                    {
                        Message = "Product updated Successfully!",
                        Success = true,
                    };
                }
                else
                {
                    Logger.LogWarning($"Validation failed while updating product with ID {id}. Errors: {string.Join(", ", validationResult.Errors.Select(e => $"{e.Field}: {e.Message}"))}");

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
                Logger.LogError(ex, "InvalidDataException occurred during Update.");
                return new ProductOperationResultDto()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred during Update.");
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
