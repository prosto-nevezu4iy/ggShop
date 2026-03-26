using CatalogService.RequestHelpers;
using FluentValidation;

namespace CatalogService.Validators.Platforms;

public class PlatformPagedFilterRequestValidator : PagedRequestValidator<PlatformPagedFilterRequest>
{
    public PlatformPagedFilterRequestValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100);
    }
}