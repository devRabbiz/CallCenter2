using CallCenter.Model.Services.DTO;
using FluentValidation;

namespace CallCenter.UI.Validators
{
    public class CallValidator : AbstractValidator<CallDetails>
    {
        public CallValidator()
        {
            RuleFor(c => c.OrderCost).GreaterThanOrEqualTo(0);
            RuleFor(c => c.CallReport).Length(5, 500).WithMessage("Длина строки должна быть от 5 до 500 символов");
        }
    }
}