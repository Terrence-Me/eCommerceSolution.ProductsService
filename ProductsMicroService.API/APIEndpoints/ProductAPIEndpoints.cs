using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using FluentValidation.Results;


namespace ProductsMicroService.API.APIEndpoints;

public static class ProductAPIEndpoints
{
    public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/products
        app.MapGet("/api/products", async (IProductsService productsService) =>
        {
            List<ProductResponse?> products = await productsService.GetProductsAsync();

            return Results.Ok(products);
        });

        // GET /api/products/search/productId/{productId}
        app.MapGet("/api/products/search/productId/{productId:guid}", async (IProductsService productsService, Guid productId) =>
        {
            ProductResponse? product = await productsService.GetProductByConditionAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(product);
        });

        // GET /api/products/search/{SearchString}
        app.MapGet("/api/products/search/{SearchString}", async (IProductsService productsService, string searchString) =>
        {
            List<ProductResponse?> products = await productsService.GetProductsByConditionAsync(p => p.ProductName != null && p.ProductName.Contains(searchString, StringComparison.OrdinalIgnoreCase) || p.Category != null && p.Category.Contains(searchString, StringComparison.OrdinalIgnoreCase));

            if (products == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(products);
        });

        // POST /api/products
        app.MapPost("/api/products", async (IProductsService productsService, IValidator<ProductAddRequest> productAddRequestValidator, ProductAddRequest productAddRequest) =>
        {
            // validate productAddRequest
            ValidationResult validationResult = await productAddRequestValidator.ValidateAsync(productAddRequest);

            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors.GroupBy(e => e.PropertyName).ToDictionary(grp => grp.Key, grp => grp.Select(e => e.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }

            ProductResponse? productResponse = await productsService.AddProductAsync(productAddRequest);

            if (productResponse == null)
            {
                return Results.Problem(" Error adding product");
            }

            return Results.Created($"/api/products/search/productId/{productResponse.ProductId}", productResponse);
        });

        // PUT /api/products
        app.MapPut("/api/products", async (IProductsService productsService, IValidator<ProductUpdateRequest> productUpdateRequestValidator, ProductUpdateRequest productUpdateRequest) =>
        {
            // validate productUpdateRequest
            ValidationResult validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

            if (!validationResult.IsValid)
            {
                Dictionary<string, string[]> errors = validationResult.Errors.GroupBy(e => e.PropertyName).ToDictionary(grp => grp.Key, grp => grp.Select(e => e.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }

            ProductResponse? UpatedProductResponse = await productsService.UpdateProductAsync(productUpdateRequest);

            if (UpatedProductResponse == null)
            {
                return Results.Problem("Error updating product");
            }

            return Results.Ok(UpatedProductResponse);
        });

        // DELETE /api/products/{productId} 
        app.MapDelete("/api/products/{productId:guid}", async (IProductsService productsService, Guid productId) =>
        {
            bool isDeleted = await productsService.DeleteProductAsync(productId);

            if (!isDeleted)
            {
                return Results.Problem("Error deleting product");
            }

            return Results.NoContent();
        });

        return app;
    }
}
