using CatalogService.DTOs.Games;
using FluentValidation;
using static CatalogService.Errors.GameErrors;

namespace CatalogService.Validators.Games;

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
            .NotEmpty()
            .Must(LinkMustBeAUri)
            .WithMessage(MustBeValidUrl);

        RuleFor(x => x.TrailerUrl)
            .NotEmpty()
            .Must(LinkMustBeAUri)
            .WithMessage(MustBeValidUrl);

        RuleFor(x => x.BackgroundUrl)
            .NotEmpty()
            .Must(LinkMustBeAUri)
            .WithMessage(MustBeValidUrl);

        RuleFor(x => x.ScreenShotUrls)
            .NotEmpty()
            .ForEach(x => x.Must(LinkMustBeAUri))
            .WithMessage(MustBeValidUrl);

        RuleFor(x => x.Genres)
            .NotEmpty();
    }

    private static bool LinkMustBeAUri(string link)
    {
        if (string.IsNullOrWhiteSpace(link))
        {
            return false;
        }
        return Uri.TryCreate(link, UriKind.Absolute, out _);
    }
}
