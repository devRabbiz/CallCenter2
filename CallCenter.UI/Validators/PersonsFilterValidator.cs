using CallCenter.Model;
using FluentValidation;

namespace CallCenter.UI.Validators
{
    public class PersonsFilterValidator : AbstractValidator<PersonsFilters>
    {
        public PersonsFilterValidator()
        {
            RuleFor(f => f.Gender).IsInEnum().WithMessage("Некорректное числовое значение Gender");
            RuleFor(f => f.MaxAge).GreaterThanOrEqualTo(0).WithMessage("Некорректное значение MaxAge");
            RuleFor(f => f.MinAge).GreaterThanOrEqualTo(0).WithMessage("Некорректное значение MinAge");
            RuleFor(f => f.MinAge).LessThanOrEqualTo(f => f.MaxAge).WithMessage("MinAge не должен быть больше MaxAge");
            RuleFor(f => f.MinDaysAfterLastCall).GreaterThanOrEqualTo(0).WithMessage("Некорректное значение MinDaysAfterLastCall");
            RuleFor(f => f.PageNo).GreaterThanOrEqualTo(0).WithMessage("Некорректное значение номера страницы");
            RuleFor(f => f.PageSize).GreaterThanOrEqualTo(0).WithMessage("Некорректное значение количества записей для страницы");            
        }
    }
}