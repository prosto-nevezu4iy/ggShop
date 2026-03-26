using CatalogService.RequestHelpers;
using CatalogService.Validators.Platforms;
using FluentValidation;

namespace CatalogService.Validators.Genres;

public class GenrePagedFilterRequestValidator : PagedRequestValidator<GenrePagedFilterRequest>
{
    public GenrePagedFilterRequestValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100);
    }
}