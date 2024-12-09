using FluentValidation;
using static ProductService.ProductViewModels.ProductViewModels;

namespace ProductService.Models
{
    public class ProductValidator : AbstractValidator<ProductInputDto>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Product Name is required.")
                .MaximumLength(100).WithMessage("Product Name can't be longer than 100 characters.");

            RuleFor(p => p.Price)
                .InclusiveBetween(1, 10000).WithMessage("Price must be between 1 and 10000.");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("Description is required.");
        }
    }

}
