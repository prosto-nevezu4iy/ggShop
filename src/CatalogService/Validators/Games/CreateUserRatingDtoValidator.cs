using CatalogService.DTOs.Games;
using FluentValidation;

namespace CatalogService.Validators.Games;

public class CreateUserRatingDtoValidator : AbstractValidator<CreateUserRatingDto>
{
    public CreateUserRatingDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 100);
    }
}