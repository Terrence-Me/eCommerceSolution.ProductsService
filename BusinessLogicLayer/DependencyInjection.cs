using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Validators;
using eCommerce.ProductsService.BusinessLogicLayer.RabbitMQ;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer;
public static class DependencyInjection
{

    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ProductAddRequestToProductMappingProfile).Assembly);
        services.AddScoped<IProductsService, ProductsService>();
        services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();
        services.AddTransient<IRabbitMQPublisher, RabbitMQPublisher>();

        return services;
    }
}
