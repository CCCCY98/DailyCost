using DailyCost.Application.DTOs.Expense;
using DailyCost.Domain.Enums;
using FluentValidation;

namespace DailyCost.Application.Validators;

public sealed class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
{
    public CreateExpenseRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(9999999999m);
        RuleFor(x => x.StartDate).NotEmpty();

        When(x => x.ExpenseType != ExpenseType.FixedAsset, () =>
        {
            RuleFor(x => x.BillingCycle).NotNull();
        });
    }
}

public sealed class UpdateExpenseRequestValidator : AbstractValidator<UpdateExpenseRequest>
{
    public UpdateExpenseRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Amount).GreaterThan(0).LessThanOrEqualTo(9999999999m);
        RuleFor(x => x.StartDate).NotEmpty();
    }
}

