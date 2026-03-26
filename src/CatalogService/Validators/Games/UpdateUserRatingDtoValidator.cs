using CatalogService.DTOs.Games;
using FluentValidation;

namespace CatalogService.Validators.Games;

public class UpdateUserRatingDtoValidator : AbstractValidator<UpdateUserRatingDto>
{
    public UpdateUserRatingDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 100);
    }
}