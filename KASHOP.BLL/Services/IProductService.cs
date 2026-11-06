using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;

namespace KASHOP.BLL.Services
{
    public interface IProductService
    {
        Task<Result<ProductResponse>> CreateProduct(ProductRequest request);
        Task<Result<List<ProductResponse>>> GetAllProducts();
        Task<Result<ProductResponse>> GetProduct(Expression<Func<Product, bool>> filter);
    }
}
