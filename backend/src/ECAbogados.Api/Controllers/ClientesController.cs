using ECAbogados.Application.Casos.Queries.ListarCasos;
using ECAbogados.Application.Auth.Commands.SolicitarResetPassword;
using ECAbogados.Application.Interfaces;
using ECAbogados.Application.Mediation;
using ECAbogados.Application.Usuarios.Commands.CrearUsuario;
using ECAbogados.Application.Usuarios.Queries.ListarUsuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECAbogados.Api.Controllers;

[Authorize(Roles = "Administrador,Abogado")]
[ApiController]
[Route("api/[controller]")]
public class ClientesController(ISender sender, ICasoRepository casos) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var usuarios = await sender.Send(new ListarUsuariosQuery());
        return Ok(usuarios.Where(x => x.Rol == "Cliente"));
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearClienteRequest request)
    {
        var caso = await casos.GetByIdAsync(request.CasoId);
        if (caso is null) return BadRequest(new { message = "El expediente no existe." });
        var id = await sender.Send(new CrearUsuarioCommand(request.Email, request.PasswordTemporal, request.Nombre, "Cliente"));
        await casos.AsignarClienteYEtapaAsync(request.CasoId, id, "Integración documental");
        await sender.Send(new SolicitarResetPasswordCommand(request.Email));
        return Created(string.Empty, new { id });
    }
}

public record CrearClienteRequest(string Nombre, string Email, string PasswordTemporal, int CasoId);
