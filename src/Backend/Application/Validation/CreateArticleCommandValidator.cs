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

        if (blocksObj is not JArray blocksArray)
            blocksArray = JArray.FromObject(blocksObj); // попытка привести к массиву

        if (blocksArray.Count == 0)
            return false;

        foreach (var block in blocksArray)
        {
            var type = block["type"]?.ToString();
            var data = block["data"] as JObject;

            if (string.IsNullOrWhiteSpace(type) || data == null)
                return false;

            switch (type)
            {
                case "header":
                    if (data["level"] is null || data["level"]?.Type != JTokenType.Integer)
                        return false;
                    break;
                case "paragraph":
                    if (string.IsNullOrWhiteSpace(data["text"]?.ToString()))
                        return false;
                    break;

                case "image":
                    if (data["file"]?["url"] is null || data["file"]?["url"]?.Type != JTokenType.String)
                        return false;
                    break;

                case "video":
                    if (string.IsNullOrWhiteSpace(data["url"]?.ToString()))
                        return false;
                    break;
                default:
                    return false;
            }
        }

        return true;
    }
}
