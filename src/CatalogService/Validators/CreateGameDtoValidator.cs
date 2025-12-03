using CatalogService.DTOs;
using FluentValidation;

namespace CatalogService.Validators;

public class CreateGameDtoValidator : AbstractValidator<CreateGameDto>
{
    public CreateGameDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.FullDescription)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.SystemRequirements)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Price)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.Discount)
            .InclusiveBetween<CreateGameDto, byte>(1, 100);

        RuleFor(x => x.Platforms)
            .NotEmpty();

        RuleFor(x => x.Publisher)
            .NotEmpty();

        RuleFor(x => x.Rating)
            .InclusiveBetween<CreateGameDto, byte>(1, 100);

        RuleFor(x => x.ImageUrl)
            .NotEmpty();

        RuleFor(x => x.TrailerUrl)
            .NotEmpty();

        RuleFor(x => x.BackgroundUrl)
            .NotEmpty();

        RuleFor(x => x.ScreenShotUrls)
            .NotEmpty();

        RuleFor(x => x.Genres)
            .NotEmpty();
    }
}
