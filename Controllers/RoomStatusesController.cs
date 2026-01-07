using DTOs;
using DTOs.Graph;
using LogicaAplicacion.ServiceInterfaces;
using LogicaAplicacion.ServiceInterfaces.Graph;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Room Statuses")]
    public class RoomStatusesController : ControllerBase
    {
        private readonly IRoomStatusService _service;
        private readonly IGraphCalendarService _graphCalendar;

        public RoomStatusesController(IRoomStatusService service, IGraphCalendarService graphCalendar)
        {
            _service = service;
            _graphCalendar = graphCalendar;
        }

        /// <summary>
        /// Obtiene todos los estados de sala.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoomStatusDTO>), 200)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            return Ok(await _service.ListAsync(ct));
        }

        /// <summary>
        /// Obtiene un estado de sala por Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RoomStatusDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
        {
            var item = await _service.FindByIdAsync(id, ct);
            return item == null ? NotFound(new { message = "Estado de sala no encontrado." }) : Ok(item);
        }

        /// <summary>
        /// Crea un nuevo estado de sala.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RoomStatusDTO), 201)]
        public async Task<IActionResult> Create([FromBody] RoomStatusDTO dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Actualiza un estado de sala existente.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(RoomStatusDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] RoomStatusDTO dto, CancellationToken ct)
        {
            dto.Id = id;
            var updated = await _service.UpdateAsync(dto, ct);
            return updated == null ? NotFound(new { message = "Estado de sala no encontrado." }) : Ok(updated);
        }

        /// <summary>
        /// Elimina un estado de sala por Id.
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
                return NotFound(new { message = "Estado de sala no encontrado." });
            }
            catch (InvalidOperationException)
            {
                return NotFound(new { message = "Estado de sala no encontrado." });
            }

            return NoContent();
        }


        #region Graph
        [HttpPost("graph/schedule")]
        public async Task<ActionResult<IReadOnlyList<ScheduleResultDto>>> GetSchedule([FromBody] ScheduleRequestDto dto, CancellationToken ct)
       => Ok(await _graphCalendar.GetScheduleAsync(dto, ct));
        #endregion
    }
}
