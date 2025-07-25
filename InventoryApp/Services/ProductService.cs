using InventoryApp.Interfaces;
using InventoryApp.Models;
using InventoryApp.Validators;
using InventoryApp.Dtos;

namespace InventoryApp.Services
{
    internal class ProductService : IProductService
    {
        private readonly IFileService fileService;
        private readonly ProductValidator validator;
        public ProductService(string path)
        {
            fileService = new FileService(path);
            validator = new ProductValidator();
        }
        public Result Create(Product product)
        {
            try
            {
                var validatorResult = validator.Validate(product);
                if (validatorResult.IsValid)
                {
                    var products = fileService.ReadFromFile();
                    products.Add(product);
                    fileService.WriteToFile(products);
                    return new Result()
                    {
                        Message = "Product Created Successfully!",
                        Success = true,
                    };
                }
                else
                {
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
                return new Result()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception)
            {
                return new Result()
                {
                    Message = "An unexpected error occurred.",
                    Success = false,
                };
            }
        }

        public DeleteResult Delete(Guid id)
        {
            try
            {
                var products = fileService.ReadFromFile();
                var product = products.FirstOrDefault(p => p.Id == id);
                if (product is null)
                {
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
                    return new DeleteResult()
                    {
                        Message = "Product deleted Successfully!",
                        Success = true,
                    };
                }

            }
            catch (Exception ex) when (ex is InvalidDataException)
            {
                return new DeleteResult()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception)
            {
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
                var products = GetProducts(filter).Products;
                Product? product = null;
                if (products is not null)
                {
                    product = products.FirstOrDefault();
                }
                else
                {
                    return new GetProductResult()
                    {
                        Message = "Failed to retrieve products from file."
                    };
                }
                return product is null ?
                new GetProductResult()
                {
                    Message = "Product Not Found.",
                }
                :
                new GetProductResult()
                {
                    Product = product,
                };
            }
            catch (Exception)
            {
                return new GetProductResult()
                {
                    Message = "An unexpected error occurred."
                };
            }
        }

        public GetProductsResult GetProducts(Func<Product, bool>? filter)
        {
            try
            {
                var products = fileService.ReadFromFile();
                if(filter is not null)
                {
                    products = products.Where(filter).ToList();
                }
                return new GetProductsResult()
                {
                    Products = products,
                    Message = null
                };
            }
            catch(Exception ex) when (ex is InvalidDataException)
            {
                return new GetProductsResult() { 
                    Products = null,
                    Message = "Something went wrong, could be Invalid file format.",

                };
            }
            
            catch(Exception)
            {
                return new GetProductsResult()
                {
                    Products = null,
                    Message = "An unexpected error occurred."
                };
            }
        }

        public Result Update(Guid id, Product product)
        {
            try
            {
                var products = fileService.ReadFromFile();
                var oldProduct = products.FirstOrDefault(p => p.Id == id);
                if(oldProduct is null) {
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
                    return new Result()
                    {
                        Message = "Product updated Successfully!",
                        Success = true,
                    };
                }
                else
                {
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
                return new Result()
                {
                    Message = "Something went wrong, could be Invalid file format.",
                    Success = false,
                };
            }

            catch (Exception)
            {
                return new Result()
                {
                    Message = "An unexpected error occurred.",
                    Success = false,
                };
            }
        }
    }
}
