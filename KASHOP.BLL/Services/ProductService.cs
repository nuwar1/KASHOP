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

        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public ProductService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService; 
        }

        public async Task<Result<ProductResponse>> CreateProduct(ProductRequest request)
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

            await _unitOfWork.ProductRepository.CreateAsync(product);
            await _unitOfWork.CompleteAsync();

            return Result<ProductResponse>.OK(product.Adapt<ProductResponse>(), "Product created successfully");
        }

        public async Task<Result<List<ProductResponse>>> GetAllProducts()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                new string[] { nameof(Product.ProductTranslations), nameof(Product.Category) });

            return Result<List<ProductResponse>>.OK(products.Adapt<List<ProductResponse>>());
        }

        public async Task<Result<ProductResponse>> GetProduct(
            Expression<Func<Product, bool>> filter)
        {
            var product = await _unitOfWork.ProductRepository.GetOne(filter,
                          new string[] { nameof(Product.ProductTranslations), nameof(Product.Category) });

            if (product is null)
            {
                return Result<ProductResponse>.Fail("Product not found");
            }

            return Result<ProductResponse>.OK(product.Adapt<ProductResponse>());
        }
    }
}
