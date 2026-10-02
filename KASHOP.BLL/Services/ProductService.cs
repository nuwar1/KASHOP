using System;
using System.Collections.Generic;
using System.Linq;
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
                if(request.MainImage is null)
                {
                    return new Result<ProductResponse>()
                    {
                        Success = false,
                        Message = "Main image is required"
                    };
                }

                var uploadResult = await _fileService.UploadAsync(request.MainImage);
                if (!uploadResult.Success)
                {
                    return new Result<ProductResponse>()
                    {
                        Success = false,
                        Message = uploadResult.Message
                    };
                }

                var product = request.Adapt<Product>();
                product.MainImage = uploadResult.Data;
                await _productRepository.CreateAsync(product);
                return new Result<ProductResponse>()
                {
                    Success = true,
                    Message = "Success",
                };
            }catch (Exception ex) {
                return new Result<ProductResponse>()
                {
                    Success = false,
                    Message = ex.InnerException.Message
                };
            }
        }
    }
}
