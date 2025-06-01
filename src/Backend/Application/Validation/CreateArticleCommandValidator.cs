using System.Reflection.Metadata.Ecma335;
using Application.Articles.Commands;
using FluentValidation;
using Newtonsoft.Json.Linq;

public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title is too long");

        RuleFor(x => x.CreatorId)
            .NotEqual(Guid.Empty).WithMessage("CreatorId is invalid");

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

        if (blocksObj is not Array array)
            return false;

        dynamic[] blocksArray = array.Cast<dynamic>().ToArray();
        foreach (dynamic block in blocksArray)
        {
            string type = block.type?.ToString() ?? string.Empty;
            dynamic? data = block.data;

            if (string.IsNullOrWhiteSpace(type) || data == null)
                return false;

            switch (type)
            {
                case "header":
                    if (data!.level is null || !int.TryParse(data.level?.ToString(), out int _))
                        return false;
                    break;
                case "paragraph":
                    if (string.IsNullOrWhiteSpace(data!.text?.ToString()))
                        return false;
                    break;
                case "image" or "video":
                    string url = data!.file?.url.url?.ToString() ?? string.Empty;
                    if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Absolute, out var _))
                        return false;
                    break;
                default:
                    return false;
            }
        }

        return true;
    }
}
