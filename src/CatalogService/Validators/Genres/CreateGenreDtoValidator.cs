using CatalogService.DTOs.Genres;
using FluentValidation;

namespace CatalogService.Validators.Genres;

public class CreateGenreDtoValidator : AbstractValidator<CreateGenreDto>
{
    public CreateGenreDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}