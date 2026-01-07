using DTOs;
using DTOs.Graph;
using LogicaAplicacion.ServiceInterfaces;
using LogicaAplicacion.ServiceInterfaces.Graph;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Reservations")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IGraphCalendarService _graphCalendar;
        private readonly IGraphCheckInService _graphCheckIn;

        public ReservationController(IReservationService reservationService, IGraphCalendarService graphCalendar, IGraphCheckInService graphCheckIn)
        {
            _reservationService = reservationService;
            _graphCalendar = graphCalendar;
            _graphCheckIn = graphCheckIn;
        }

        /// <summary>
        /// Crea una nueva reserva.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ReservationDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<ReservationDTO>> Create([FromBody] ReservationCreateDTO dto, CancellationToken ct)
        {
            try
            {
                var created = await _reservationService.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las reservas.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReservationDTO>), 200)]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAll(CancellationToken ct)
        {
            var results = await _reservationService.ListAsync(ct);
            return Ok(results);
        }

        /// <summary>
        /// Obtiene una reserva por Id.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReservationDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ReservationDTO>> GetById(int id, CancellationToken ct)
        {
            var reservation = await _reservationService.FindByIdAsync(id, ct);
            return reservation == null
                ? NotFound(new { message = "Reserva no encontrada." })
                : Ok(reservation);
        }

        /// <summary>
        /// Obtiene reservas por Id de usuario.
        /// </summary>
        [HttpGet("user/{userId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ReservationDTO>), 200)]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetByUser(
            int userId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? order,
            CancellationToken ct)
        {
            var results = await _reservationService.ListByUserAsync(userId, startDate, endDate, order, ct);
            return Ok(results);
        }

        /// <summary>
        /// Obtiene reservas por Id de sede.
        /// </summary>
        [HttpGet("location/{locationId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ReservationDTO>), 200)]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetByLocation(int locationId, CancellationToken ct)
        {
            var results = await _reservationService.ListByLocationAsync(locationId, ct);
            return Ok(results);
        }

        /// <summary>
        /// Obtiene reservas por Id de sala.
        /// </summary>
        [HttpGet("room/{roomId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ReservationDTO>), 200)]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetByRoom(int roomId, CancellationToken ct)
        {
            var results = await _reservationService.ListByRoomAsync(roomId, ct);
            return Ok(results);
        }

        /// <summary>
        /// Actualiza una reserva existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ReservationDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<ReservationDTO>> Update(int id, [FromBody] ReservationDTO dto, CancellationToken ct)
        {
            if (dto.Id != id)
                return BadRequest(new { message = "El id del body no coincide con el id de la ruta." });

            try
            {
                var updated = await _reservationService.UpdateAsync(dto, ct);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancela una reserva.
        /// </summary>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(ReservationDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ReservationDTO>> Cancel(
            int id,
            [FromQuery] int? cancelledByUserId = null,
            [FromQuery] string? cancellationReason = null,
            CancellationToken ct = default)
        {
            try
            {
                var canceled = await _reservationService.CancelAsync(id, cancelledByUserId, cancellationReason, ct);
                return Ok(canceled);
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
        /// Elimina una reserva.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                await _reservationService.DeleteAsync(id, ct);
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
        /// Realiza check-in de una reserva.
        /// </summary>
        [HttpPost("{id}/check-in")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CheckIn(int id, CancellationToken ct)
        {
            try
            {
                await _reservationService.CheckInAsync(id, DateTime.UtcNow, ct);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Realiza check-out de una reserva.
        /// </summary>
        [HttpPost("{id}/check-out")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CheckOut(int id, CancellationToken ct)
        {
            try
            {
                await _reservationService.CheckOutAsync(id, DateTime.UtcNow, ct);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Extiende la duración de una reserva con valor por defecto.
        /// </summary>
        [HttpPost("{id}/extend-default")]
        [ProducesResponseType(typeof(ReservationDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<ReservationDTO>> ExtendDefault(int id, CancellationToken ct)
        {
            try
            {
                var extended = await _reservationService.ExtendDefaultAsync(id, ct);
                return Ok(extended);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }


        #region Graph
        [HttpPost("graph")]
        public async Task<ActionResult<ReservationCreateResultDto>> CreateGraphReservation(
      [FromBody] ReservationCreateRequestDto dto,
      CancellationToken ct)
        {
            var result = await _graphCalendar.CreateReservationAsync(dto, ct);
            return Ok(result);
        }

        [HttpDelete("graph/{eventId}")]
        public async Task<IActionResult> DeleteGraphReservation(string eventId, CancellationToken ct)
        {
            await _graphCalendar.DeleteReservationAsync(eventId, ct);
            return NoContent();
        }

        [HttpPost("graph/checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto dto, CancellationToken ct)
        {
            await _graphCheckIn.CreateCheckInAsync(dto, ct);
            return NoContent();
        }
        #endregion
    }
}
