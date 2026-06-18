using FluentValidation;

namespace IndaloAventurApi.Application.Features.WordPress.GetWordPressPostBySlug;

public sealed class GetWordPressPostBySlugQueryValidator : AbstractValidator<GetWordPressPostBySlugQuery>
{
    public GetWordPressPostBySlugQueryValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("El slug del post es obligatorio.");
    }
}
