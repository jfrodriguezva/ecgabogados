using ECAbogados.Application.Mediation;

namespace ECAbogados.Application.Documentos.Commands.SubirDocumento;

public record SubirDocumentoCommand(
    int CasoId,
    string NombreArchivo,
    string TipoContenido,
    long TamanoBytes,
    byte[] Contenido) : IRequest<int>;
