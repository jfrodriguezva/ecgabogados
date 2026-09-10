using System.Security.Claims;
using ECAbogados.Application.Interfaces;
using ECAbogados.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECAbogados.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/casos/{casoId:int}/mensajes")]
public class ConversacionesController(ICasoRepository casos, IMensajeExpedienteRepository mensajes) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(int casoId)
    {
        if (!await PuedeAcceder(casoId)) return Forbid();
        return Ok(await mensajes.GetByCasoIdAsync(casoId));
    }

    [HttpPost]
    public async Task<IActionResult> Enviar(int casoId, [FromBody] EnviarMensajeRequest request)
    {
        if (!await PuedeAcceder(casoId)) return Forbid();
        if (string.IsNullOrWhiteSpace(request.Mensaje) || request.Mensaje.Length > 2000)
            return BadRequest(new { message = "El mensaje debe contener entre 1 y 2000 caracteres." });
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var item = new MensajeExpediente
        {
            CasoId = casoId,
            UsuarioId = usuarioId,
            AutorNombre = User.FindFirstValue(ClaimTypes.Name) ?? "Usuario",
            AutorRol = User.FindFirstValue(ClaimTypes.Role) ?? "Usuario",
            Mensaje = request.Mensaje.Trim(),
            FechaEnvio = DateTime.UtcNow
        };
        var id = await mensajes.CreateAsync(item);
        return Created(string.Empty, new { id });
    }

    private async Task<bool> PuedeAcceder(int casoId)
    {
        if (!User.IsInRole("Cliente")) return User.IsInRole("Administrador") || User.IsInRole("Abogado") || User.IsInRole("Asistente");
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var caso = await casos.GetByIdAsync(casoId);
        return caso?.ClienteUsuarioId == usuarioId;
    }
}

public record EnviarMensajeRequest(string Mensaje);
