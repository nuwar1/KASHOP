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
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateCategory(CategoryRequest request);
        Task<List<CategoryResponse>> GetAllCategories();
        Task<CategoryResponse> GetCategory(Expression<Func<Category, bool>> filter);
        Task<bool> DeleteCategory(int id);
        Task<CategoryResponse> UpdateCategory(int id, CategoryRequest request);
    }
}
