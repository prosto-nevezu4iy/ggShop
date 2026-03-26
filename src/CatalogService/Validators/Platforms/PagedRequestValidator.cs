using Common.Application.Requests.Pagination;
using FluentValidation;

namespace CatalogService.Validators.Platforms;

public class PagedRequestValidator<T> : AbstractValidator<T> where T : PagedRequest
{
    public PagedRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .InclusiveBetween(1, 99);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 99);
    }
}