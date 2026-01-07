using LogicaNegocio.Dominio.Notifications;
using LogicaNegocio.InterfacesRepositorios;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Outbox")]
    public class OutboxController : ControllerBase
    {
        private readonly IOutboxRepository _repo;

        public OutboxController(IOutboxRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Obtiene todos los mensajes de outbox.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OutboxMessage>), 200)]
        public async Task<ActionResult<IEnumerable<OutboxMessage>>> GetAll(CancellationToken ct)
        {
            var items = await _repo.GetAllAsync(ct);
            return Ok(items);
        }

        /// <summary>
        /// Obtiene un mensaje de outbox por Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OutboxMessage), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OutboxMessage>> GetById(Guid id, CancellationToken ct)
        {
            var m = await _repo.GetByIdAsync(id, ct);
            return m is null 
                ? NotFound(new { message = "Outbox message no encontrada." }) 
                : Ok(m);
        }
    }
}
