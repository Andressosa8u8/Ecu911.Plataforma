using Ecu911.AuthService.DTOs;
using FluentValidation;

namespace Ecu911.AuthService.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MinimumLength(4).WithMessage("El nombre de usuario debe tener al menos 4 caracteres.")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede superar los 50 caracteres.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MinimumLength(5).WithMessage("El nombre completo debe tener al menos 5 caracteres.")
            .MaximumLength(150).WithMessage("El nombre completo no puede superar los 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El correo electrónico no puede superar los 150 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número.");
    }
}