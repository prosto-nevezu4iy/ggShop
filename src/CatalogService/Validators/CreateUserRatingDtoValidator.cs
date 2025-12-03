using CatalogService.DTOs;
using FluentValidation;

namespace CatalogService.Validators;

public class CreateUserRatingDtoValidator : AbstractValidator<CreateUserRatingDto>
{
    public CreateUserRatingDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 100);
    }
}