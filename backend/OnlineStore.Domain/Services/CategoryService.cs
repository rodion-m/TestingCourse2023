﻿using OnlineStore.Domain.Entities;
using OnlineStore.Domain.RepositoryInterfaces;

namespace OnlineStore.Domain.Services;

public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public virtual async Task<IReadOnlyCollection<Category>> GetCategories(CancellationToken cancellationToken) =>
        await _unitOfWork.CategoryRepository.GetAll(cancellationToken);

    public virtual async Task<Category> GetCategory(Guid id, CancellationToken cancellationToken) =>
        await _unitOfWork.CategoryRepository.GetById(id, cancellationToken);

    public virtual async Task<Category> AddCategory(string name, CancellationToken cancellationToken)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var category = new Category(Guid.NewGuid(), name);
        await _unitOfWork.CategoryRepository.Add(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category;
    }

    public virtual async Task<Category> UpdateCategory(string name, CancellationToken cancellationToken)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        var category = await _unitOfWork.CategoryRepository.GetByName(name, cancellationToken);
        await _unitOfWork.CategoryRepository.Update(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category;
    }

    public virtual async Task<Category> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.CategoryRepository.DeleteById(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category;
    }
}