using CatalogService.RequestHelpers;
using CatalogService.Validators.Platforms;
using FluentValidation;

namespace CatalogService.Validators.Games;

public class GamePagedFilterRequestValidator : PagedRequestValidator<GamePagedFilterRequest>
{
    public GamePagedFilterRequestValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100);
    }
}