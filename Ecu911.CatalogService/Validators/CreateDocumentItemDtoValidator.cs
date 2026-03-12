using Ecu911.CatalogService.DTOs;
using FluentValidation;

namespace Ecu911.CatalogService.Validators;

public class CreateDocumentItemDtoValidator : AbstractValidator<CreateDocumentItemDto>
{
    public CreateDocumentItemDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MinimumLength(3).WithMessage("El título debe tener al menos 3 caracteres.")
            .MaximumLength(200).WithMessage("El título no puede superar los 200 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MinimumLength(5).WithMessage("La descripción debe tener al menos 5 caracteres.")
            .MaximumLength(1000).WithMessage("La descripción no puede superar los 1000 caracteres.");
    }
}