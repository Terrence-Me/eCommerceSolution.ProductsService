using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators;
public class ProductAddRequestValidator : AbstractValidator<ProductAddRequest>
{
    public ProductAddRequestValidator()
    {
        RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product Name cannot be blank");
        RuleFor(x => x.Category).IsInEnum().WithMessage("Category cannot be blank");
        RuleFor(x => x.UnitPrice).InclusiveBetween(0, double.MaxValue);
        RuleFor(x => x.QuantityInStock).GreaterThan(0).WithMessage($"Unit Price should be between 0 and {double.MaxValue}");
        RuleFor(x => x.QuantityInStock).InclusiveBetween(0, int.MaxValue).WithMessage($"Quantity in Stock should be between 0 and {int.MaxValue}");
    }
}
