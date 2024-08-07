﻿using eID.PAN.Contracts.Commands;
using FluentValidation;

namespace eID.PAN.Service.Validators;

public class GetSystemsByFilterValidator : AbstractValidator<GetSystemsByFilter>
{
    public GetSystemsByFilterValidator()
    {
        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
        RuleFor(r => r.RegisteredSystemState).IsInEnum();
    }
}
