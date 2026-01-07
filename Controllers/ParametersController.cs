using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Parameters")]
    public class ParametersController : ControllerBase
    {
        private readonly IParameterService _service;

        public ParametersController(IParameterService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene todos los parámetros.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ParameterDTO>), 200)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _service.ListAsync(ct);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un parámetro por su Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ParameterDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
        {
            var result = await _service.FindByIdAsync(id, ct);
            return result == null ? NotFound(new { message = "Parámetro no encontrado." }) : Ok(result);
        }

        /// <summary>
        /// Crea un nuevo parámetro (operación no permitida).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(403)]
        public Task<IActionResult> Create([FromBody] ParameterDTO dto, CancellationToken ct)
        {
            var message = "Los parámetros del sistema sólo se pueden definir desde el código fuente.";
            IActionResult response = StatusCode(403, new { message });
            return Task.FromResult(response);
        }

        /// <summary>
        /// Actualiza un parámetro existente.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ParameterDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] ParameterDTO dto, CancellationToken ct)
        {
            dto.Id = id;

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Datos inválidos." });

            try
            {
                var updated = await _service.UpdateAsync(dto, ct);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Parámetro no encontrado." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un parámetro por su Id.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _service.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Parámetro no encontrado." });
            }
        }
    }
}
