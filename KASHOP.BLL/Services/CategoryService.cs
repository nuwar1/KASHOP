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
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService (IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest request)
        {
            var category = request.Adapt<Category>();

            await _unitOfWork.CategoryRepository.CreateAsync(category);
            await _unitOfWork.CompleteAsync();

            return Result<CategoryResponse>.Ok(category.Adapt<CategoryResponse>());
        }
        public async Task<Result<List<CategoryResponse>>> GetAllCategories()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync(
            new string[] { nameof(Category.CategoryTranslations) }
            );

            return Result<List<CategoryResponse>>.Ok(categories.Adapt<List<CategoryResponse>>());
        }
        public async Task<Result<CategoryResponse>> GetCategory(
                    Expression<Func<Category, bool>> filter)
        {
            var category = await _unitOfWork.CategoryRepository.GetOne(filter,
                           new string[] { nameof(Category.CategoryTranslations) });

            if (category is null)
            {
                return Result<CategoryResponse>.Fail("Category not found");
            }

            return Result<CategoryResponse>.Ok(category.Adapt<CategoryResponse>());
        }
        public async Task<Result<bool>> DeleteCategory(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetOne(c => c.Id == id);

            if (category is null)
            {
                return Result<bool>.Fail("Category not found");
            }

            _unitOfWork.CategoryRepository.Delete(category);
            var affectedRows = await _unitOfWork.CompleteAsync();

            if (affectedRows == 0)
            {
                return Result<bool>.Fail("Failed to delete category");
            }

            return Result<bool>.Ok(true, "Category deleted successfully");
        }

        public async Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
        {
            var category = await _unitOfWork.CategoryRepository.GetOne(c => c.Id == id, 
                new string[] { nameof(Category.CategoryTranslations) });

            if (category is null)
            {
                return Result<CategoryResponse>.Fail("Category not found");
            }

            request.Adapt(category);

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.CompleteAsync();

            return Result<CategoryResponse>.Ok(category.Adapt<CategoryResponse>(), "Category updated successfully");
        }
    }
}
