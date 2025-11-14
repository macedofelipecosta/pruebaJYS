using AutoMapper;
using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    /// <summary>
    /// Controlador responsable de la gestión de salas.
    /// </summary>
    /// <remarks>
    /// Proporciona operaciones CRUD básicas: crear, listar, buscar por ID y eliminar salas.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Salas")]
    public class RoomsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRoomService _roomService;

        /// <summary>
        /// Constructor del controlador de salas.
        /// </summary>
        /// <param name="mapper">Instancia del mapper de AutoMapper.</param>
        /// <param name="roomService">Servicio de dominio para la gestión de salas.</param>
        public RoomsController(IMapper mapper, IRoomService roomService)
        {
            _mapper = mapper;
            _roomService = roomService;
        }

        /// <summary>
        /// Crea una nueva sala.
        /// </summary>
        /// <param name="objDTO">Datos de la sala a crear.</param>
        /// <returns>La sala creada.</returns>
        /// <response code="201">La sala fue creada exitosamente.</response>
        /// <response code="400">Error en los datos enviados.</response>
        [HttpPost]
        [ProducesResponseType(typeof(RoomDTO), 201)]
        [ProducesResponseType(400)]
        public ActionResult<RoomDTO> Create(RoomDTO objDTO)
        {
            try
            {
                var created = _roomService.Create(objDTO);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una sala por su identificador único.
        /// </summary>
        /// <param name="id">Identificador de la sala.</param>
        /// <returns>La sala encontrada, si existe.</returns>
        /// <response code="200">Sala encontrada.</response>
        /// <response code="404">No se encontró una sala con el ID especificado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoomDTO), 200)]
        [ProducesResponseType(404)]
        public ActionResult<RoomDTO> GetById(int id)
        {
            var room = _roomService.FindById(id);
            if (room == null)
                return NotFound(new { message = "Sala no encontrada." });

            var dto = _mapper.Map<RoomDTO>(room);
            return Ok(dto);
        }

        /// <summary>
        /// Obtiene la lista completa de salas registradas.
        /// </summary>
        /// <returns>Una lista de todas las salas.</returns>
        /// <response code="200">Lista obtenida exitosamente.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoomDTO>), 200)]
        public ActionResult<IEnumerable<RoomDTO>> GetAll()
        {
            var rooms = _roomService.List();
            var dtoList = _mapper.Map<IEnumerable<RoomDTO>>(rooms);
            return Ok(dtoList);
        }

        /// <summary>
        /// Actualiza los datos de una sala existente.
        /// </summary>
        /// <param name="id">ID de la sala a actualizar.</param>
        /// <param name="dto">Datos actualizados de la sala.</param>
        /// <returns>Sala actualizada.</returns>
        /// <response code="200">Sala actualizada exitosamente.</response>
        /// <response code="400">Datos inválidos o error en la operación.</response>
        /// <response code="404">No existe una sala con el ID indicado.</response>
        [HttpPut]
        [ProducesResponseType(typeof(RoomDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<RoomDTO> Update(RoomDTO dto)
        {
            try
            {
                var updated = _roomService.Update(dto);

                if (updated == null)
                    return NotFound(new { message = "No existe una sala con ese ID." });

                var result = _mapper.Map<RoomDTO>(updated);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina una sala existente por su ID.
        /// </summary>
        /// <param name="id">Identificador de la sala a eliminar.</param>
        /// <response code="204">Sala eliminada exitosamente.</response>
        /// <response code="400">Ocurrió un error al eliminar la sala.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public ActionResult Delete(int id)
        {
            try
            {
                _roomService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
