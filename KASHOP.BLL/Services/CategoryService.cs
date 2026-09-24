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
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace KASHOP.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService (ICategoryRepository categoryRepository)
        {
           _categoryRepository = categoryRepository;
        }

        public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest request)
        {
            try
            {
                var category = request.Adapt<Category>();
                await _categoryRepository.CreateAsync(category);

                return new Result<CategoryResponse>
                {
                    Success = true,
                    Message = "Success",
                    Data = category.Adapt<CategoryResponse>()
                };
            }
            catch(Exception ex)
            {
                return new Result<CategoryResponse>
                {
                    Success = false,
                    Message = ex.InnerException.Message
                };
            }
        }

        public async Task<Result<List<CategoryResponse>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync(
                new string[] { nameof(Category.CategoryTranslations) }
                );

                return new Result<List<CategoryResponse>>
                {
                    Success = true,
                    Message = "Success",
                    Data = categories.Adapt<List<CategoryResponse>>()
                };
            } catch (Exception ex) {
                return new Result<List<CategoryResponse>>() {
                    Success = false,
                    Message = ex.InnerException.Message
                };
            }
        }

        public async Task<Result<CategoryResponse>> GetCategory(Expression<Func<Category, bool>> filter)
        {
            try
            {
                var category = await _categoryRepository.GetOne(filter, new string[] { nameof(Category.CategoryTranslations) });
                if (category is null) {
                    return new Result<CategoryResponse>
                    {
                        Success = false,
                        Message = "Category Not Found"
                    };
                }
                return new Result<CategoryResponse>
                {
                    Success = true,
                    Message = "Success",
                    Data = category.Adapt<CategoryResponse>()
                };
            }
            catch (Exception ex) {
                return new Result<CategoryResponse>
                {
                    Success = false,
                    Message = ex.InnerException.Message
                };
            } 
        }
        public async Task<Result<bool>> DeleteCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetOne(c => c.Id == id);
                if (category is null)
                {
                    return new Result<bool>
                    {
                        Success = false,
                        Message = "Category Not Found",
                        Data = false
                    };
                }
                var deleted = await _categoryRepository.DeleteAsync(category);
                return new Result<bool>
                {
                    Success = deleted,
                    Message = deleted? "Success" : "Faild to delete category",
                    Data = deleted
                };
            }
            catch(Exception ex)
            {
                return new Result<bool>
                {
                    Success = false,
                    Message = ex.InnerException.Message,
                    Data = false
                };
            }
        }
                
        public async Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
        {
            try
            {
                var category = await _categoryRepository.GetOne(c => c.Id == id, new string[] { nameof(Category.CategoryTranslations) });

                if (category is null)
                {
                    return new Result<CategoryResponse>
                    {
                        Success = false,
                        Message = "Category Not Found"
                    };
                }
                request.Adapt(category);
                var updatedCategory = await _categoryRepository.UpdateAsync(category);
                return new Result<CategoryResponse>
                {
                    Success = true,
                    Message = "Success",
                    Data = updatedCategory.Adapt<CategoryResponse>()
                };
            }
            catch(Exception ex)
            {
                return new Result<CategoryResponse>
                {
                    Success = false,
                    Message = ex.InnerException.Message
                };
            }
            
        }
    }
}
