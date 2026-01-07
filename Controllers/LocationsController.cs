using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    /// <summary>
    /// Controlador para la gestión de sedes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Locations")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Crea una nueva sede.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(LocationDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<LocationDTO>> Create([FromBody] LocationDTO dto, CancellationToken ct)
        {
            try
            {
                var created = await _locationService.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una sede por Id.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LocationDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<LocationDTO>> GetById(int id, CancellationToken ct)
        {
            var location = await _locationService.FindByIdAsync(id, ct);
            return location == null 
                ? NotFound(new { message = "Sede no encontrada." }) 
                : Ok(location);
        }

        /// <summary>
        /// Obtiene todas las sedes.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LocationDTO>), 200)]
        public async Task<ActionResult<IEnumerable<LocationDTO>>> GetAll(CancellationToken ct)
        {
            var list = await _locationService.ListAsync(ct);
            return Ok(list);
        }

        /// <summary>
        /// Actualiza una sede existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LocationDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<LocationDTO>> Update(int id, [FromBody] LocationDTO dto, CancellationToken ct)
        {
            if (dto.Id != id)
                return BadRequest(new { message = "El id del body no coincide con el id de la ruta." });

            try
            {
                var updated = await _locationService.UpdateAsync(dto, ct);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una sede por Id.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _locationService.DeleteAsync(id, ct);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el teléfono de una sede.
        /// </summary>
        [HttpPatch("{id}/phone")]
        [ProducesResponseType(typeof(LocationDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<LocationDTO>> UpdatePhone(int id, [FromBody] UpdatePhoneRequest request, CancellationToken ct)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PhoneNumber))
                return BadRequest(new { message = "El teléfono no puede estar vacío." });
            
            try
            {
                var updated = await _locationService.UpdatePhoneAsync(id, request.PhoneNumber, ct);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        public class UpdatePhoneRequest
        {
            public string PhoneNumber { get; set; } = string.Empty;
        }
    }
}
