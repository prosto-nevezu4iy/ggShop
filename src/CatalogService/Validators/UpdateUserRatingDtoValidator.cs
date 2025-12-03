using CatalogService.DTOs;
using FluentValidation;

namespace CatalogService.Validators;

public class UpdateUserRatingDtoValidator : AbstractValidator<UpdateUserRatingDto>
{
    public UpdateUserRatingDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 100);
    }
}