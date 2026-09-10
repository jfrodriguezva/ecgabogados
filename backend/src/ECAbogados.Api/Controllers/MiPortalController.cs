using ECAbogados.Application.Interfaces;
using ECAbogados.Application.Documentos;
using ECAbogados.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECAbogados.Api.Controllers;

[Authorize(Roles = "Cliente")]
[ApiController]
[Route("api/mi-portal")]
public class MiPortalController(
    ICurrentUserAccessor currentUser,
    ICasoRepository casos,
    IChecklistItemRepository checklist,
    IDocumentoRepository documentos,
    IPlazoRepository plazos,
    IPagoRepository pagos,
    IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet("casos")]
    public async Task<IActionResult> MisCasos()
    {
        if (currentUser.UsuarioId is not int usuarioId)
            return Unauthorized();

        var propios = await casos.GetByClienteUsuarioIdAsync(usuarioId);
        var resultado = new List<object>();
        foreach (var caso in propios)
        {
            var requisitos = await checklist.GetByCasoIdAsync(caso.Id);
            var archivos = await documentos.GetByCasoIdAsync(caso.Id);
            var fechas = await plazos.GetByCasoIdAsync(caso.Id);
            var movimientos = await pagos.GetByCasoIdAsync(caso.Id);
            resultado.Add(new
            {
                caso.Id,
                caso.ClienteNombre,
                caso.Tipo,
                caso.Estatus,
                caso.Etapa,
                caso.FechaApertura,
                checklist = requisitos,
                documentos = archivos.Select(x => new { x.Id, x.NombreArchivo, x.TipoContenido, x.TamanoBytes, x.FechaCarga }),
                fechas,
                pagos = movimientos,
                totalPagado = movimientos.Sum(x => x.Monto)
            });
        }
        return Ok(resultado);
    }

    [HttpPost("casos/{casoId:int}/documentos")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> SubirDocumento(int casoId, IFormFile file)
    {
        if (currentUser.UsuarioId is not int usuarioId) return Unauthorized();
        var caso = await casos.GetByIdAsync(casoId);
        if (caso?.ClienteUsuarioId != usuarioId) return Forbid();
        if (file.Length == 0 || file.Length > TiposPermitidos.TamanoMaximoBytes || !TiposPermitidos.EsExtensionPermitida(file.FileName))
            return BadRequest(new { message = "Archivo vacío, demasiado grande o de un tipo no permitido." });
        var carpeta = Path.Combine(environment.ContentRootPath, "App_Data", "documentos", casoId.ToString());
        Directory.CreateDirectory(carpeta);
        var nombre = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var ruta = Path.Combine(carpeta, nombre);
        await using (var stream = new FileStream(ruta, FileMode.CreateNew))
            await file.CopyToAsync(stream);
        var id = await documentos.CreateAsync(new Documento
        {
            CasoId = casoId,
            NombreArchivo = Path.GetFileName(file.FileName),
            TipoContenido = file.ContentType,
            TamanoBytes = file.Length,
            FechaCarga = DateTime.UtcNow,
            RutaAlmacenamiento = Path.Combine("App_Data", "documentos", casoId.ToString(), nombre)
        });
        return Created(string.Empty, new { id });
    }

    [HttpGet("casos/{casoId:int}/documentos/{documentoId:int}/archivo")]
    public async Task<IActionResult> DescargarDocumento(int casoId, int documentoId)
    {
        if (currentUser.UsuarioId is not int usuarioId) return Unauthorized();
        var caso = await casos.GetByIdAsync(casoId);
        if (caso?.ClienteUsuarioId != usuarioId) return Forbid();
        var documento = await documentos.GetByIdAsync(documentoId);
        if (documento is null || documento.CasoId != casoId) return NotFound();

        var raiz = Path.GetFullPath(environment.ContentRootPath) + Path.DirectorySeparatorChar;
        var ruta = Path.GetFullPath(Path.Combine(environment.ContentRootPath, documento.RutaAlmacenamiento));
        if (!ruta.StartsWith(raiz, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(ruta)) return NotFound();
        return PhysicalFile(ruta, documento.TipoContenido ?? "application/octet-stream", documento.NombreArchivo, enableRangeProcessing: true);
    }
}
