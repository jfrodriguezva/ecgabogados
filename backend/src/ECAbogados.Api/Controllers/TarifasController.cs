using ECAbogados.Application.Interfaces;
using ECAbogados.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ECAbogados.Api.Controllers;
[Authorize(Roles = "Administrador,Abogado")]
[ApiController]
[Route("api/[controller]")]
public class TarifasController(ITarifaRepository tarifas) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await tarifas.GetAllAsync());
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Tarifa x)
    {
        if (string.IsNullOrWhiteSpace(x.Area) || string.IsNullOrWhiteSpace(x.Servicio) || string.IsNullOrWhiteSpace(x.Concepto) || x.MontoBase < 0) return BadRequest();
        return Created(string.Empty, new { id = await tarifas.CreateAsync(x) });
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] Tarifa x)
    {
        x.Id = id;
        await tarifas.UpdateAsync(x);
        return NoContent();
    }
}
