using CallCenter.Model.Services.DTO;
using FluentValidation;

namespace CallCenter.UI.Validators
{
    public class PersonValidator : AbstractValidator<PersonDetails>
    {
        public PersonValidator()
        {
            RuleFor(p => p.FirstName).NotNull().WithMessage("Обязательное поле");
            RuleFor(p => p.FirstName).Length(3, 30).WithMessage("Должно быть от 3 до 30 символов");
            RuleFor(p => p.PhoneNumber).NotNull().WithMessage("Обязательное поле");
            RuleFor(p => p.PhoneNumber).Length(5, 20).WithMessage("Должно быть от 5 до 20 символов");
            RuleFor(p => p.LastName).Length(0, 30).WithMessage("Не должно быть больше 30 символов");
            RuleFor(p => p.Patronymic).Length(0, 30).WithMessage("Не должно быть больше 30 символов");
            RuleFor(p => p.Gender).IsInEnum();
        }                
    }
}