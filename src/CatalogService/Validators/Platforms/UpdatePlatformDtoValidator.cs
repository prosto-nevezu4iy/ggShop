using CatalogService.DTOs.Platforms;
using FluentValidation;

namespace CatalogService.Validators.Platforms;

public class UpdatePlatformDtoValidator : AbstractValidator<UpdatePlatformDto>
{
    public UpdatePlatformDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}