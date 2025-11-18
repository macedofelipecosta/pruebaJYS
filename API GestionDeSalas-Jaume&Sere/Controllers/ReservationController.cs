using AutoMapper;
using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    /// <summary>
    /// Controlador que expone la API REST para la gestión de reservas
    /// </summary>
    /// <remarks>
    /// Este controlador recibe solicitudes HTTP, delega el procesamiento en la capa
    /// de servicios y retorna respuestas serializadas (DTOs). Forma parte de una
    /// arquitectura por capas.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Reservas")]
    public class ReservationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IReservationService _reservationService;

        public ReservationController(IMapper mapper, IReservationService reservationService)
        {
            _mapper = mapper;
            _reservationService = reservationService;
        }

        /// <summary>
        /// Crea una nueva reserva.
        /// </summary>
        /// <param name="reservationDto">DTO con los datos de la reserva.</param>
        /// <returns>Reserva creada.</returns>
        /// <response code="201">La reserva fue creada correctamente.</response>
        /// <response code="400">Los datos enviados no son válidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ReservationDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ReservationDTO>> Create([FromBody] ReservationDTO reservationDto, CancellationToken cancellationToken)
        {
            try
            {
                var created = await _reservationService.CreateAsync(reservationDto, cancellationToken);

                return CreatedAtAction(
                    nameof(GetReservationById),
                    new { id = created.Id }, created
                    );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las reservas registradas.
        /// </summary>
        /// <returns>Lista de reservas.</returns>
        /// <response code="200">Operación exitosa.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReservationDTO>), 200)]
        public ActionResult<IEnumerable<ReservationDTO>> GetAll()
        {
            try
            {
                var reservations = _reservationService.List();
                return Ok(_mapper.Map<IEnumerable<ReservationDTO>>(reservations));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una reserva según su ID.
        /// </summary>
        /// <param name="id">ID de la reserva.</param>
        /// <returns>Reserva encontrada.</returns>
        /// <response code="200">Reserva encontrada.</response>
        /// <response code="404">No existe una reserva con ese ID.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReservationDTO), 200)]
        [ProducesResponseType(404)]
        public ActionResult<ReservationDTO> GetReservationById(int id)
        {
            try
            {
                var reservation = _reservationService.FindById(id);
                if (reservation == null)
                    return NotFound();

                return Ok(_mapper.Map<ReservationDTO>(reservation));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una reserva existente.
        /// </summary>
        /// <param name="id">ID de la reserva a actualizar.</param>
        /// <param name="reservationDto">Datos actualizados.</param>
        /// <returns>Reserva actualizada.</returns>
        /// <response code="200">Reserva actualizada correctamente.</response>
        /// <response code="400">Datos inválidos.</response>
        /// <response code="404">No se encontró la reserva.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ReservationDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<ReservationDTO> Update([FromBody] ReservationDTO reservationDto)
        {
            try
            {
                var updated = _reservationService.Update(reservationDto);

                if (updated == null)
                    return NotFound();

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una reserva por su ID.
        /// </summary>
        /// <param name="id">ID de la reserva a eliminar.</param>
        /// <returns>Resultado de la operación.</returns>
        /// <response code="204">Reserva eliminada correctamente.</response>
        /// <response code="404">No existe una reserva con ese ID.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Delete(int id)
        {
            try
            {
                _reservationService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
