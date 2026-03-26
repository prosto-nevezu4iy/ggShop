using CatalogService.RequestHelpers;
using CatalogService.Validators.Platforms;
using FluentValidation;

namespace CatalogService.Validators.Publishers;

public class PublisherPagedFilterRequestValidator : PagedRequestValidator<PublisherPagedFilterRequest>
{
    public PublisherPagedFilterRequestValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100);
    }
}