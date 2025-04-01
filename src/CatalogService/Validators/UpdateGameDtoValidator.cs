using CatalogService.DTOs;
using FluentValidation;

namespace CatalogService.Validators;

public class UpdateGameDtoValidator : AbstractValidator<UpdateGameDto>
{
    public UpdateGameDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.FullDescription)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Discount)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Platforms)
            .NotEmpty();

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.ScreenShotUrls)
            .NotEmpty();
    }
}
