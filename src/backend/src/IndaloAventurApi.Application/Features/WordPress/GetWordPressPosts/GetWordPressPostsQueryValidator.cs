using FluentValidation;

namespace IndaloAventurApi.Application.Features.WordPress.GetWordPressPosts;

public sealed class GetWordPressPostsQueryValidator : AbstractValidator<GetWordPressPostsQuery>
{
    public GetWordPressPostsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("La pagina debe ser mayor que 0.");
        RuleFor(x => x.PageSize!.Value)
            .InclusiveBetween(1, 100)
            .When(x => x.PageSize.HasValue)
            .WithMessage("El tamano de pagina debe estar entre 1 y 100.");
    }
}
