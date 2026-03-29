using FluentValidation;
using ShopQueue.Requests;

namespace ShopQueue.Validators;

public class CreateShopRequestValidator : AbstractValidator<CreateShopRequest>
{
    public CreateShopRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(x => x.Address).NotEmpty().MinimumLength(5).MaximumLength(200);
    }
}