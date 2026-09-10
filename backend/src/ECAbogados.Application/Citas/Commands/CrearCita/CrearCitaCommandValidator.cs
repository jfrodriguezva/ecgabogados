using FluentValidation;

namespace ECAbogados.Application.Citas.Commands.CrearCita;

public class CrearCitaCommandValidator : AbstractValidator<CrearCitaCommand>
{
    public CrearCitaCommandValidator()
    {
        RuleFor(x => x.NombreCliente).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Telefono).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Servicio).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Modalidad).NotEmpty().Must(x => x is "Videollamada" or "Llamada telefónica" or "Presencial por confirmar");
        RuleFor(x => x.Comentario).MaximumLength(1000);
        RuleFor(x => x.FechaHora).NotEmpty();
    }
}
