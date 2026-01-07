using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    /// <summary>
    /// Controlador para gestión de equipamiento.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Equipment")]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _service;

        public EquipmentController(IEquipmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene todo el equipamiento.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EquipmentDTO>), 200)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            return Ok(await _service.ListAsync(ct));
        }

        /// <summary>
        /// Obtiene equipamiento por Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EquipmentDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
        {
            var item = await _service.FindByIdAsync(id, ct);
            return item == null ? NotFound(new { message = "Equipamiento no encontrado." }) : Ok(item);
        }

        /// <summary>
        /// Crea un nuevo equipamiento.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(EquipmentDTO), 201)]
        public async Task<IActionResult> Create([FromBody] EquipmentDTO dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza un equipamiento existente.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(EquipmentDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] EquipmentDTO dto, CancellationToken ct)
        {
            dto.Id = id;
            var updated = await _service.UpdateAsync(dto, ct);
            return updated == null ? NotFound(new { message = "Equipamiento no encontrado." }) : Ok(updated);
        }

        /// <summary>
        /// Elimina equipamiento por Id.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _service.DeleteAsync(id, ct);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Equipamiento no encontrado." });
            }
            catch (InvalidOperationException)
            {
                return NotFound(new { message = "Equipamiento no encontrado." });
            }

            return NoContent();
        }
    }
}
