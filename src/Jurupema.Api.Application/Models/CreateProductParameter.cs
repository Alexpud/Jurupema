using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Jurupema.Api.Application.Models;

public class CreateProductParameter
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Product name is required")]
    public string Name { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Product description is required")]
    public string Description { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public decimal Price { get; set; }
}

public class CreateProductParameterValidator : AbstractValidator<CreateProductParameter>
{
    public CreateProductParameterValidator()
    {
        RuleFor(p => p.Name).NotEmpty();

        RuleFor(p => p.Description).NotEmpty();

        RuleFor(p => p.Price).NotEmpty();
    }
}