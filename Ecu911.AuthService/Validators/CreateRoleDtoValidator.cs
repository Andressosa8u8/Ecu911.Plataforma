using Ecu911.AuthService.DTOs;
using FluentValidation;

namespace Ecu911.AuthService.Validators;

public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
            .MinimumLength(3).WithMessage("El nombre del rol debe tener al menos 3 caracteres.")
            .MaximumLength(50).WithMessage("El nombre del rol no puede superar los 50 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción del rol es obligatoria.")
            .MinimumLength(5).WithMessage("La descripción debe tener al menos 5 caracteres.")
            .MaximumLength(200).WithMessage("La descripción no puede superar los 200 caracteres.");
    }
}