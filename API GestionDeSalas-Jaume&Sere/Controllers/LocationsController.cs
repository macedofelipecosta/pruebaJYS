using AutoMapper;
using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    /// <summary>
    /// Controlador responsable de la gestión de sedes.
    /// </summary>
    /// <remarks>
    /// Permite crear, listar, buscar y eliminar sedes registradas en el sistema.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Sedes")]
    public class LocationsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILocationService _sedeService;

        /// <summary>
        /// Constructor del controlador de sedes.
        /// </summary>
        /// <param name="mapper">Instancia de AutoMapper para conversión de objetos.</param>
        /// <param name="sedeService">Servicio de aplicación que maneja la lógica de negocio de sedes.</param>
        public LocationsController(IMapper mapper, ILocationService sedeService)
        {
            _mapper = mapper;
            _sedeService = sedeService;
        }

        /// <summary>
        /// Crea una nueva sede.
        /// </summary>
        /// <param name="dto">Datos de la sede a crear.</param>
        /// <returns>La sede creada.</returns>
        /// <response code="201">Sede creada correctamente.</response>
        /// <response code="400">Error en los datos enviados.</response>
        [HttpPost]
        [ProducesResponseType(typeof(LocationDTO), 201)]
        [ProducesResponseType(400)]
        public ActionResult<LocationDTO> Create(LocationDTO dto)
        {
            try
            {
                var creada = _sedeService.Create(dto);
                var result = _mapper.Map<LocationDTO>(creada);

                return CreatedAtAction(nameof(GetById), new { id = creada.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una sede por su ID.
        /// </summary>
        /// <param name="id">Identificador de la sede.</param>
        /// <returns>Datos de la sede solicitada.</returns>
        /// <response code="200">Sede encontrada.</response>
        /// <response code="404">No se encontró una sede con ese ID.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LocationDTO), 200)]
        [ProducesResponseType(404)]
        public ActionResult<LocationDTO> GetById(int id)
        {
            var sede = _sedeService.FindById(id);
            if (sede == null)
                return NotFound(new { message = "Sede no encontrada." });

            var dto = _mapper.Map<LocationDTO>(sede);
            return Ok(dto);
        }

        /// <summary>
        /// Obtiene la lista completa de sedes.
        /// </summary>
        /// <returns>Una lista de todas las sedes registradas.</returns>
        /// <response code="200">Lista obtenida exitosamente.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LocationDTO>), 200)]
        public ActionResult<IEnumerable<LocationDTO>> GetAll()
        {
            var sedes = _sedeService.List();
            var dtoList = _mapper.Map<IEnumerable<LocationDTO>>(sedes);
            return Ok(dtoList);
        }

        /// <summary>
        /// Actualiza los datos de una sede existente.
        /// </summary>
        /// <param name="id">ID de la sede a actualizar.</param>
        /// <param name="dto">Datos actualizados de la sede.</param>
        /// <returns>Sede actualizada.</returns>
        /// <response code="200">Sede actualizada correctamente.</response>
        /// <response code="400">Datos inválidos o inconsistentes.</response>
        /// <response code="404">No existe una sede con el ID proporcionado.</response>
        [HttpPut]
        [ProducesResponseType(typeof(LocationDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult<LocationDTO> Update(int id, LocationDTO dto)
        {
            try
            {
                var updated = _sedeService.Update(dto);
                if (updated == null)
                    return NotFound(new { message = "No existe una sede con ese ID." });

                var result = _mapper.Map<LocationDTO>(updated);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina una sede por su ID.
        /// </summary>
        /// <param name="id">Identificador de la sede a eliminar.</param>
        /// <response code="204">Sede eliminada exitosamente.</response>
        /// <response code="400">Error al intentar eliminar la sede.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public ActionResult Delete(int id)
        {
            try
            {
                _sedeService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
