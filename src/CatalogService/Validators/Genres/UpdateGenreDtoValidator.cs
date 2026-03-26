using CatalogService.DTOs.Genres;
using FluentValidation;

namespace CatalogService.Validators.Genres;

public class UpdateGenreDtoValidator : AbstractValidator<UpdateGenreDto>
{
    public UpdateGenreDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}