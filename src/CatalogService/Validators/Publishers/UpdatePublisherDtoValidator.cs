using CatalogService.DTOs.Publishers;
using FluentValidation;

namespace CatalogService.Validators.Publishers;

public class UpdatePublisherDtoValidator : AbstractValidator<UpdatePublisherDto>
{
    public UpdatePublisherDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}