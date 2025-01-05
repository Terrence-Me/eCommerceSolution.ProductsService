using DataAccessLayer.Entities;
using System.Linq.Expressions;

namespace eCommerce.DataAccessLayer.RepositoryContracts;
/// <summary>
/// repository interface for managing products
/// </summary>
public interface IProductsRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();
    /// <summary>
    /// retieves products base on speficied condition
    /// </summary>
    /// <param name="conditionExpression"> Condition to filter products</param>
    /// <returns> Returns collection of matching products</returns>
    Task<IEnumerable<Product>> GetProductsByConditionAsync(Expression<Func<Product, bool>> conditionExpression);

    Task<Product?> GetProductByConditionAsync(Expression<Func<Product, bool>> conditionExpression);
    Task<Product?> AddProductAsync(Product product);
    Task<Product?> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(Guid productId);
}
