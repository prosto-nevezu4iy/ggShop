using CatalogService.DTOs.Publishers;
using FluentValidation;

namespace CatalogService.Validators.Publishers;

public class CreatePublisherDtoValidator : AbstractValidator<CreatePublisherDto>
{
    public CreatePublisherDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}