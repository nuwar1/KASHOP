using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;

namespace KASHOP.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        public ProductService(IProductRepository productRepository, IFileService fileService)
        {
            _productRepository = productRepository;
            _fileService = fileService;
        }

        public async Task<Result<ProductResponse>> CreateProduct(ProductRequest request)
        {
            try
            {
                if (request.MainImage is null)
                {
                    return Result<ProductResponse>.Fail("Main image is required");
                }

                var uploadResult = await _fileService.UploadAsync(request.MainImage);

                if (!uploadResult.Success)
                {
                    return Result<ProductResponse>.Fail(uploadResult.Message);
                }

                var product = request.Adapt<Product>();
                product.MainImage = uploadResult.Data;

                await _productRepository.CreateAsync(product);

                return Result<ProductResponse>.OK(product.Adapt<ProductResponse>(), "Product created successfully");
            }
            catch (Exception ex)
            {
                return Result<ProductResponse>.Fail(ex.InnerException.Message);
            }
        }

        public async Task<Result<List<ProductResponse>>> GetAllProducts()
        {
            try
            {
                var products = await _productRepository.GetAllAsync(
                    new string[]{nameof(Product.ProductTranslations),nameof(Product.Category)});

                return Result<List<ProductResponse>>.OK(products.Adapt<List<ProductResponse>>());
            }
            catch (Exception ex)
            {
                return Result<List<ProductResponse>>.Fail(ex.InnerException.Message);
            }
        }

        public async Task<Result<ProductResponse>> GetProduct(
            Expression<Func<Product, bool>> filter)
        {
            try
            {
                var product = await _productRepository.GetOne(filter,
                    new string[] {nameof(Product.ProductTranslations), nameof(Product.Category)});

                if (product is null)
                {
                    return Result<ProductResponse>.Fail("Product not found");
                }

                return Result<ProductResponse>.OK(product.Adapt<ProductResponse>());
            }
            catch (Exception ex)
            {
                return Result<ProductResponse>.Fail(ex.InnerException.Message);
            }
        }
    }
}
