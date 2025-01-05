using DataAccessLayer.Entities;
using eCommerce.DataAccessLayer.Context;
using eCommerce.DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace eCommerce.DataAccessLayer.Repositories;
public class ProductsRepository(ApplicationDbContext dbContext) : IProductsRepository
{
    private ApplicationDbContext _dbContext = dbContext;

    public async Task<Product?> AddProductAsync(Product product)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        Product? existingProdcut = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

        if (existingProdcut == null)
        {
            return false;
        }

        _dbContext.Products.Remove(existingProdcut);
        int affectedRowsCount = await _dbContext.SaveChangesAsync();
        return affectedRowsCount > 0;
    }

    public async Task<Product?> GetProductByConditionAsync(Expression<Func<Product, bool>> conditionExpression)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(conditionExpression);
    }

    public async Task<IEnumerable<Product>> GetProductsByConditionAsync(Expression<Func<Product, bool>> conditionExpression)
    {
        return await _dbContext.Products.Where(conditionExpression).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {

        return await _dbContext.Products.ToListAsync();

    }

    public async Task<Product?> UpdateProductAsync(Product product)
    {
        Product? existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

        if (existingProduct == null)
        {
            return null;
        }
        else
        {
            existingProduct.ProductName = product.ProductName;
            existingProduct.Category = product.Category;
            existingProduct.UnitPrice = product.UnitPrice;
            existingProduct.QuantityInStock = product.QuantityInStock;

            await _dbContext.SaveChangesAsync();
            return existingProduct;
        }
    }
}
