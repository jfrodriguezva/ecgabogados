using ECAbogados.Application.Documentos;
using ECAbogados.Application.Documentos.Commands.SubirDocumento;
using ECAbogados.Application.Documentos.Queries.ListarDocumentosPorCaso;
using ECAbogados.Application.Interfaces;
using ECAbogados.Application.Mediation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECAbogados.Api.Controllers;

[Authorize(Roles = "Administrador,Abogado,Asistente")]
[ApiController]
[Route("api/[controller]")]
public class DocumentosController(ISender sender, IDocumentoRepository documentos) : ControllerBase
{
    [HttpGet("{id:int}/archivo")]
    public async Task<IActionResult> Descargar(int id)
    {
        var documento = await documentos.GetByIdAsync(id);
        if (documento is null) return NotFound();
        if (documento.Contenido is null) return NotFound();
        return File(documento.Contenido, documento.TipoContenido ?? "application/octet-stream", documento.NombreArchivo, enableRangeProcessing: true);
    }

    [HttpGet("caso/{casoId:int}")]
    public async Task<IActionResult> ListarPorCaso(int casoId)
    {
        var documentos = await sender.Send(new ListarDocumentosPorCasoQuery(casoId));
        return Ok(documentos);
    }

    [HttpPost]
    [RequestSizeLimit(TiposPermitidos.TamanoMaximoBytes)]
    public async Task<IActionResult> Subir([FromForm] SubirDocumentoRequest request)
    {
        var casoId = request.CasoId;
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

        var command = new SubirDocumentoCommand(
            casoId,
            file.FileName,
            file.ContentType,
            file.Length,
            stream.ToArray());

        var id = await sender.Send(command);

        return CreatedAtAction(nameof(ListarPorCaso), new { casoId }, new { id });
    }

}

public class SubirDocumentoRequest
{
    public int CasoId { get; set; }
    public IFormFile File { get; set; } = null!;
}
