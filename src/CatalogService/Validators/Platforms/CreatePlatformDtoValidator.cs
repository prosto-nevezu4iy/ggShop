using CatalogService.DTOs.Platforms;
using FluentValidation;

namespace CatalogService.Validators.Platforms;

public class CreatePlatformDtoValidator : AbstractValidator<CreatePlatformDto>
{
    public CreatePlatformDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}