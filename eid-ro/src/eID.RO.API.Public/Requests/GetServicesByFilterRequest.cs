﻿using FluentValidation;

namespace eID.RO.API.Public.Requests;

public class GetServicesByFilterRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetServicesByFilterRequestValidator();
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    public int? ServiceNumber { get; set; }
    /// <summary>
    /// Searches the contents of Name for the given string
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Searches the contents of Description for the given string
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Searches the service number and name for the given string
    /// </summary>
    public string? FindServiceNumberAndName { get; set; }
    /// <summary>
    /// Set to 'true' to return only services that can be empowered.
    /// </summary>
    public bool IncludeEmpowermentOnly { get; set; } = true;
    /// <summary>
    /// Set to 'true' and the result will contain soft-deleted records.
    /// </summary>
    public bool IncludeDeleted { get; set; } = false;
    public Guid? BatchId { get; set; }
    public Guid? SectionId { get; set; }
}

internal class GetServicesByFilterRequestValidator : AbstractValidator<GetServicesByFilterRequest>
{
    public GetServicesByFilterRequestValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
        RuleFor(r => r.Name).MaximumLength(128)
            .When(r => !string.IsNullOrEmpty(r.Name), ApplyConditionTo.CurrentValidator);
        RuleFor(r => r.Description).MaximumLength(512)
            .When(r => !string.IsNullOrEmpty(r.Description), ApplyConditionTo.CurrentValidator);
        RuleFor(r => r.FindServiceNumberAndName).MaximumLength(128)
            .When(r => !string.IsNullOrEmpty(r.FindServiceNumberAndName), ApplyConditionTo.CurrentValidator);
    }
}
