﻿using Application.Articles.Queries;
using FluentValidation;

namespace Application.Validation;

public class GetArticlesQueryValidator : AbstractValidator<GetArticlesQuery>
{
    public static readonly string[] ValidSortTypes = ["asc", "desc", "none"];
    public GetArticlesQueryValidator()
    {
        RuleFor(article => article.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than 1");

        RuleFor(article => article.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be greater than 1");

        RuleFor(article => article.AuthorId)
            .Must(id => id != Guid.Empty)
            .When(article => article.AuthorId is not null)
            .WithMessage("Invalid author id format. Id must be Guid.");

        RuleFor(article => article.SortByDate)
            .Must(IsSortTypeValid!)
            .When(article => article.SortByDate is not null)
            .WithMessage("Invalid sort type. Valid types: {asc, desc, 'none'}");

        RuleFor(article => article.SortByPopularity)
            .Must(IsSortTypeValid!)
            .When(article => article.SortByPopularity is not null)
            .WithMessage("Invalid sort type. Valid types: {asc, desc, 'none'}");
    }

    private bool IsSortTypeValid(string type)
        => ValidSortTypes.Contains(type.ToLower());
}
