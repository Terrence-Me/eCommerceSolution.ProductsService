using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using eCommerce.DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services;
public class ProductsService(IProductsRepository productsRepository, IMapper mapper, IValidator<ProductAddRequest> productAddRequestValidator, IValidator<ProductUpdateRequest> productUpdateRequest, IRabbitMQPublisher rabbitMQPublisher) : IProductsService
{
    private readonly IProductsRepository _productsRepository = productsRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<ProductAddRequest> _productAddRequestValidator = productAddRequestValidator;
    private readonly IValidator<ProductUpdateRequest> _productUpdateRequest = productUpdateRequest;

    public async Task<ProductResponse?> AddProductAsync(ProductAddRequest productAddRequest)
    {
        ArgumentNullException.ThrowIfNull(productAddRequest);

        ValidationResult validationResult = await _productAddRequestValidator.ValidateAsync(productAddRequest);

        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errors);
        }

        Product product = _mapper.Map<Product>(productAddRequest);

        Product? addedProduct = await _productsRepository.AddProductAsync(product);

        if (addedProduct == null)
        {
            return null;
        }

        return _mapper.Map<ProductResponse>(addedProduct);
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        ArgumentNullException.ThrowIfNull(productId);

        Product? existingProduct = await _productsRepository.GetProductByConditionAsync(p => p.ProductId == productId);

        if (existingProduct == null)
        {
            return false;
        }
        bool isDeleted = await _productsRepository.DeleteProductAsync(productId);

        return isDeleted;
    }

    public async Task<ProductResponse?> GetProductByConditionAsync(Expression<Func<Product, bool>> conditionExpression)
    {
        ArgumentNullException.ThrowIfNull(conditionExpression);

        Product? product = await _productsRepository.GetProductByConditionAsync(conditionExpression);

        if (product == null)
        {
            return null;
        }

        return _mapper.Map<ProductResponse>(product);

    }

    public async Task<List<ProductResponse?>> GetProductsAsync()
    {


        IEnumerable<Product> products = await _productsRepository.GetProductsAsync();

        IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products); //Invokes ProductToProductResponseMappingProfile
        return productResponses.ToList();


        //return _mapper.Map<List<ProductResponse?>>(products);
    }

    public async Task<List<ProductResponse?>> GetProductsByConditionAsync(Expression<Func<Product, bool>> conditionExpression)
    {
        ArgumentNullException.ThrowIfNull(conditionExpression);

        IEnumerable<Product> products = await _productsRepository.GetProductsByConditionAsync(conditionExpression);

        return _mapper.Map<List<ProductResponse?>>(products);
    }

    public async Task<ProductResponse?> UpdateProductAsync(ProductUpdateRequest productUpdateRequest)
    {
        ArgumentNullException.ThrowIfNull(productUpdateRequest);

        ValidationResult validationResult = await _productUpdateRequest.ValidateAsync(productUpdateRequest);

        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errors);
        }

        Product product = _mapper.Map<Product>(productUpdateRequest);

        Product? existingProduct = await _productsRepository.GetProductByConditionAsync(p => p.ProductId == product.ProductId);
        if (existingProduct == null)
        {
            throw new ArgumentNullException("Invalid Product ID");
        }

        // Check if product name changed
        bool isProductNameChanged = existingProduct.ProductName != product.ProductName;

        Product? updatedProduct = await _productsRepository.UpdateProductAsync(product);

        if (isProductNameChanged)
        {
            // Publish message to RabbitMQ
            var routingKey = "product.update.name";
            var message = new ProductNameUpdateMessage(product.ProductId, product.ProductName);
            rabbitMQPublisher.Publish(routingKey, message);
        }

        ProductResponse productResponse = _mapper.Map<ProductResponse>(updatedProduct);

        return productResponse;

    }
}
