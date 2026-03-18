using Ecu911.CatalogService.DTOs;
using FluentValidation;

namespace Ecu911.CatalogService.Validators;

public class UpdateDocumentTypeDtoValidator : AbstractValidator<UpdateDocumentTypeDto>
{
    public UpdateDocumentTypeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(300).WithMessage("La descripción no puede superar los 300 caracteres.");
    }
}