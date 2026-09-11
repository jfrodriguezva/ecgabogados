using ECAbogados.Application.Casos.Queries.ObtenerCasoPorToken;
using ECAbogados.Application.Documentos;
using ECAbogados.Application.Documentos.Commands.SubirDocumento;
using ECAbogados.Application.Documentos.Queries.ListarDocumentosPorCaso;
using ECAbogados.Application.Mediation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECAbogados.Api.Controllers;

/// <summary>
/// Portal del cliente: acceso público mediante un enlace mágico (token único por
/// caso), sin registro ni contraseña. El staff comparte el link por WhatsApp/correo.
/// </summary>
[Authorize(Roles = "Administrador,Abogado,Asistente")]
[EnableRateLimiting("public")]
[ApiController]
[Route("api/[controller]")]
public class PortalController(ISender sender) : ControllerBase
{
    [HttpGet("{token}")]
    public async Task<IActionResult> ObtenerCaso(string token)
    {
        var caso = await sender.Send(new ObtenerCasoPorTokenQuery(token));
        return caso is null ? NotFound() : Ok(caso);
    }

    [HttpPost("{token}/documentos")]
    [RequestSizeLimit(TiposPermitidos.TamanoMaximoBytes)]
    public async Task<IActionResult> SubirDocumento(string token, [FromForm] SubirDocumentoPortalRequest request)
    {
        var caso = await sender.Send(new ObtenerCasoPorTokenQuery(token));
        if (caso is null)
        {
            return NotFound();
        }

        var file = request.File;
        if (file.Length == 0)
        {
            return BadRequest(new { message = "El archivo está vacío." });
        }

        if (!TiposPermitidos.EsExtensionPermitida(file.FileName))
        {
            return BadRequest(new { message = $"Tipo de archivo no permitido. Extensiones válidas: {TiposPermitidos.ExtensionesPermitidasTexto}." });
        }

        if (file.Length > TiposPermitidos.TamanoMaximoBytes)
        {
            return BadRequest(new { message = "El archivo excede el tamaño máximo permitido (5 MB)." });
        }

        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        var id = await sender.Send(new SubirDocumentoCommand(
            caso.Id,
            file.FileName,
            file.ContentType,
            file.Length,
            stream.ToArray()));

        return Created(string.Empty, new { id });
    }
}

public class SubirDocumentoPortalRequest
{
    public IFormFile File { get; set; } = null!;
}
