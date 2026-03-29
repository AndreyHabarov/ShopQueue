using FluentValidation;
using ShopQueue.Requests;

namespace ShopQueue.Validators;

public class CreateQueueRequestValidator : AbstractValidator<CreateQueueRequest>
{
    public CreateQueueRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
    }
}