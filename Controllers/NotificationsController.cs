using DTOs;
using LogicaNegocio.InterfacesRepositorios;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _repo;

        public NotificationsController(INotificationRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Obtiene todas las notificaciones.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificationDTO>), 200)]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetAll(CancellationToken ct)
        {
            var items = await _repo.GetAllAsync(ct);
            var dtos = items.Select(n => new NotificationDTO
            {
                Id = n.Id,
                UserId = n.UserId,
                ReservationId = n.ReservationId,
                Subject = n.Subject,
                Message = n.Message,
                Sent = n.Sent,
                SentAt = n.SentAt,
                ErrorDetail = n.ErrorDetail
            });
            return Ok(dtos);
        }

        /// <summary>
        /// Obtiene una notificación por Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(NotificationDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<NotificationDTO>> GetById(int id, CancellationToken ct)
        {
            var n = await _repo.GetByIdAsync(id, ct);
            if (n is null) return NotFound(new { message = "Notificación no encontrada." });
            var dto = new NotificationDTO
            {
                Id = n.Id,
                UserId = n.UserId,
                ReservationId = n.ReservationId,
                Subject = n.Subject,
                Message = n.Message,
                Sent = n.Sent,
                SentAt = n.SentAt,
                ErrorDetail = n.ErrorDetail
            };
            return Ok(dto);
        }
    }
}
