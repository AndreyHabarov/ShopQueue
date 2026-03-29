using FluentValidation;
using ShopQueue.Requests;

namespace ShopQueue.Validators;

public class JoinQueueRequestValidator : AbstractValidator<JoinQueueRequest>
{
    public JoinQueueRequestValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CustomerPhone)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Invalid phone number format");
    }
}