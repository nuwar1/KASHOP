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

                return Result<CategoryResponse>.OK(category.Adapt<CategoryResponse>());
            }
            catch (Exception ex)
            {
                return Result<CategoryResponse>.Fail(ex.InnerException.Message);
            }
        }
        public async Task<Result<List<CategoryResponse>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync(
                    new string[]{nameof(Category.CategoryTranslations)}
                );

                return Result<List<CategoryResponse>>.OK(categories.Adapt<List<CategoryResponse>>());
            }
            catch (Exception ex)
            {
                return Result<List<CategoryResponse>>.Fail(ex.InnerException.Message);
            }
        }
        public async Task<Result<CategoryResponse>> GetCategory(
                    Expression<Func<Category, bool>> filter)
        {
            try
            {
                var category = await _categoryRepository.GetOne(filter,
                    new string[]{nameof(Category.CategoryTranslations)});

                if (category is null)
                {
                    return Result<CategoryResponse>.Fail("Category not found");
                }

                return Result<CategoryResponse>.OK(category.Adapt<CategoryResponse>());
            }
            catch (Exception ex)
            {
                return Result<CategoryResponse>.Fail(ex.InnerException.Message);
            }
        }
        public async Task<Result<bool>> DeleteCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetOne(c => c.Id == id);

                if (category is null)
                {
                    return Result<bool>.Fail("Category not found");
                }

                var deleted = await _categoryRepository.DeleteAsync(category);

                if (!deleted)
                {
                    return Result<bool>.Fail("Failed to delete category");
                }

                return Result<bool>.OK(true,"Category deleted successfully");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(ex.InnerException.Message);
            }
        }

        public async Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
        {
            try
            {
                var category = await _categoryRepository.GetOne(c => c.Id == id, new string[] {nameof(Category.CategoryTranslations)});

                if (category is null)
                {
                    return Result<CategoryResponse>.Fail("Category not found");
                }

                request.Adapt(category);

                var updatedCategory = await _categoryRepository.UpdateAsync(category);

                return Result<CategoryResponse>.OK(updatedCategory.Adapt<CategoryResponse>(),"Category updated successfully");
            }
            catch(Exception ex)
            {
                return Result<CategoryResponse>.Fail(ex.InnerException.Message);
            }
            
        }
    }
}
