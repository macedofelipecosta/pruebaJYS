using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;


        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;

        }

        /// <summary>
        /// Crea una nueva sala.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RoomDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RoomDTO>> Create([FromBody] RoomDTO dto, CancellationToken ct)
        {
            return await ExecuteWithExceptionHandlingAsync<RoomDTO>(
                async () =>
                {
                    var created = await _roomService.CreateAsync(dto, ct);
                    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
                });
        }

        /// <summary>
        /// Obtiene una sala por Id.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoomDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RoomDTO>> GetById(int id, CancellationToken ct)
        {
            return await ExecuteWithExceptionHandlingAsync<RoomDTO>(
                async () =>
                {
                    var room = await _roomService.FindByIdAsync(id, ct);
                    return room == null ? NotFound(new { message = "Sala no encontrada." }) : Ok(room);
                });
        }

        /// <summary>
        /// Lista todas las salas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoomDTO>), 200)]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAll(CancellationToken ct)
        {
            return await ExecuteWithExceptionHandlingAsync<IEnumerable<RoomDTO>>(
                async () => Ok(await _roomService.ListAsync(ct)));
        }

        /// <summary>
        /// Lista las salas por Id de sede.
        /// </summary>
        [HttpGet("by-location/{locationId:int}")]
        [ProducesResponseType(typeof(IEnumerable<RoomDTO>), 200)]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetByLocation(int locationId, CancellationToken ct)
        {
            return await ExecuteWithExceptionHandlingAsync<IEnumerable<RoomDTO>>(
                async () => Ok(await _roomService.GetByLocationAsync(locationId, ct)));
        }

        /// <summary>
        /// Lista las salas por Id de equipamiento en todas las sedes.
        /// </summary>
        [HttpGet("by-equipment")]
        [ProducesResponseType(typeof(IEnumerable<RoomDTO>), 200)]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetByEquipment([FromQuery] int[] equipmentIds, CancellationToken ct)
        {
            return await ExecuteWithExceptionHandlingAsync<IEnumerable<RoomDTO>>(
                async () =>
                {
                    if (equipmentIds == null || equipmentIds.Length == 0)
                        return BadRequest(new { message = "Debe indicar al menos un equipmentId como query param." });
                    return Ok(await _roomService.GetByEquipmentAsync(equipmentIds, ct));
                });
        }

        /// <summary>
        /// Lista las salas por Id de sede y uno o más equipamientos.
        /// </summary>
        [HttpGet("by-location/{locationId:int}/by-equipment")]
        [ProducesResponseType(typeof(IEnumerable<RoomDTO>), 200)]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetByLocationAndEquipment(int locationId, [FromQuery] int[] equipmentIds, CancellationToken ct)
        {
            return await ExecuteWithExceptionHandlingAsync<IEnumerable<RoomDTO>>(
                async () =>
                {
                    if (equipmentIds == null || equipmentIds.Length == 0)
                        return BadRequest(new { message = "Debe indicar al menos un equipmentId como query param." });
                    return Ok(await _roomService.GetByLocationAndEquipmentAsync(locationId, equipmentIds, ct));
                });
        }

        /// <summary>
        /// Actualiza una sala existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RoomDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RoomDTO>> Update(int id, [FromBody] RoomDTO dto, CancellationToken ct)
        {
            if (dto.Id != id)
                return BadRequest(new { message = "El id del body no coincide con el id de la ruta." });

            return await ExecuteWithExceptionHandlingAsync<RoomDTO>(
                async () => Ok(await _roomService.UpdateAsync(dto, ct)));
        }

        /// <summary>
        /// Elimina una sala por Id.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            return await ExecuteWithExceptionHandlingAsync(
                async () =>
                {
                    await _roomService.DeleteAsync(id, ct);
                    return NoContent();
                });
        }

        /// <summary>
        /// Cambia el estado de una sala.
        /// </summary>
        [HttpPatch("{roomId}/status/{newStatusId:int}")]
        [ProducesResponseType(typeof(RoomDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RoomDTO>> ChangeStatus(int roomId, int newStatusId, CancellationToken ct)
        {
            return await ExecuteWithExceptionHandlingAsync<RoomDTO>(
                async () => Ok(await _roomService.ChangeStatusAsync(roomId, newStatusId, ct)));
        }

        private async Task<ActionResult<T>> ExecuteWithExceptionHandlingAsync<T>(Func<Task<ActionResult<T>>> action)
        {
            try
            {
                return await action();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor.", details = ex.Message });
            }
        }

        private async Task<ActionResult> ExecuteWithExceptionHandlingAsync(Func<Task<ActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor.", details = ex.Message });
            }
        }
    }
}
