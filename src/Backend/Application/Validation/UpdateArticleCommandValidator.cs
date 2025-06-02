using Application.Articles.Commands;
using FluentValidation;
using Newtonsoft.Json.Linq;

namespace Application.Validation;

public class UpdateArticleCommandValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title is too long");

        RuleFor(x => x.Tags)
            .NotNull().WithMessage("Tags must be provided")
            .Must(tags => tags.Any()).WithMessage("At least one tag is required")
            .ForEach(tag =>
                tag.NotEmpty().WithMessage("Tag cannot be empty")
                   .MaximumLength(50).WithMessage("Tag is too long"));

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content is required")
            .Must(BeValidEditorJsContent).WithMessage("Content must be valid Editor.js format");
    }

    private bool BeValidEditorJsContent(Dictionary<string, object> content)
    {
        if (!content.TryGetValue("blocks", out var blocksObj))
            return false;

        return blocksObj is JArray; // ToDo better validation
    }
}
