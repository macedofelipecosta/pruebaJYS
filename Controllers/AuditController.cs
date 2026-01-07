using LogicaAplicacion.ServiceInterfaces;
using LogicaNegocio.InterfacesRepositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("AuditPolicy")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditRepository _auditRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<AuditController> _logger;

        public AuditController(
            IAuditRepository auditRepository,
            IAuditService auditService,
            ILogger<AuditController> logger)
        {
            _auditRepository = auditRepository;
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de auditoría.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            if (page < 1)
                return BadRequest("El número de página debe ser mayor o igual a 1");

            pageSize = Math.Min(Math.Max(pageSize, 1), 100);
            _logger.LogWarning("ACCESO AUDITORÍA: Usuario consultó auditorías - Página: {Page}, Tamaño: {PageSize}", page, pageSize);

            var allAudits = await _auditRepository.GetAllAsync(ct);
            var totalCount = allAudits.Count();
            var audits = allAudits.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            await _auditService.LogAsync(
                action: "AccessAudit",
                entityType: "Audit",
                entityId: null,
                details: new { Action = "QueryAll", Page = page, PageSize = pageSize, TotalCount = totalCount, ReturnedCount = audits.Count },
                ct: ct);

            return Ok(new
            {
                Data = audits,
                Pagination = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    HasPreviousPage = page > 1,
                    HasNextPage = page * pageSize < totalCount
                }
            });
        }

        /// <summary>
        /// Obtiene auditorías por Id de usuario.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            if (userId <= 0)
                return BadRequest("El ID de usuario debe ser mayor a 0");
            if (page < 1)
                return BadRequest("El número de página debe ser mayor o igual a 1");

            pageSize = Math.Min(Math.Max(pageSize, 1), 100);
            _logger.LogWarning("ACCESO AUDITORÍA: Consultando auditorías del usuario {UserId}", userId);

            var allAudits = await _auditRepository.GetByUserIdAsync(userId, ct);
            var totalCount = allAudits.Count();
            var audits = allAudits.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            await _auditService.LogAsync(
                action: "AccessAudit",
                entityType: "Audit",
                entityId: userId,
                details: new { Action = "QueryByUser", UserId = userId, Page = page, PageSize = pageSize, Count = audits.Count },
                ct: ct);

            return Ok(new
            {
                Data = audits,
                Pagination = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }

        /// <summary>
        /// Obtiene auditorías por rango de fechas.
        /// </summary>
        [HttpGet("daterange")]
        public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            if (from > to)
                return BadRequest("La fecha 'from' debe ser menor o igual a 'to'");
            if (page < 1)
                return BadRequest("El número de página debe ser mayor o igual a 1");

            pageSize = Math.Min(Math.Max(pageSize, 1), 100);
            _logger.LogWarning("ACCESO AUDITORÍA: Consultando auditorías entre {From} y {To}", from, to);

            var allAudits = await _auditRepository.GetByDateRangeAsync(from, to, ct);
            var totalCount = allAudits.Count();
            var audits = allAudits.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            await _auditService.LogAsync(
                action: "AccessAudit",
                entityType: "Audit",
                entityId: null,
                details: new { Action = "QueryByDateRange", From = from, To = to, Page = page, PageSize = pageSize, Count = audits.Count },
                ct: ct);

            return Ok(new
            {
                Data = audits,
                Pagination = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }

        /// <summary>
        /// Obtiene auditorías por acción.
        /// </summary>
        [HttpGet("action/{action}")]
        public async Task<IActionResult> GetByAction(
            string action,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(action))
                return BadRequest("La acción no puede estar vacía");
            if (page < 1)
                return BadRequest("El número de página debe ser mayor o igual a 1");

            pageSize = Math.Min(Math.Max(pageSize, 1), 100);
            _logger.LogWarning("ACCESO AUDITORÍA: Consultando auditorías con acción {Action}", action);

            var allAudits = await _auditRepository.GetByActionAsync(action, ct);
            var totalCount = allAudits.Count();
            var audits = allAudits.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            await _auditService.LogAsync(
                action: "AccessAudit",
                entityType: "Audit",
                entityId: null,
                details: new { Action = "QueryByAction", SearchAction = action, Page = page, PageSize = pageSize, Count = audits.Count },
                ct: ct);

            return Ok(new
            {
                Data = audits,
                Pagination = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }

        /// <summary>
        /// Obtiene auditorías por tipo de entidad.
        /// </summary>
        [HttpGet("entity/{entityType}")]
        public async Task<IActionResult> GetByEntityType(
            string entityType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(entityType))
                return BadRequest("El tipo de entidad no puede estar vacío");
            if (page < 1)
                return BadRequest("El número de página debe ser mayor o igual a 1");

            pageSize = Math.Min(Math.Max(pageSize, 1), 100);
            _logger.LogWarning("ACCESO AUDITORÍA: Consultando auditorías del tipo de entidad {EntityType}", entityType);

            var allAudits = await _auditRepository.GetByEntityTypeAsync(entityType, ct);
            var totalCount = allAudits.Count();
            var audits = allAudits.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            await _auditService.LogAsync(
                action: "AccessAudit",
                entityType: "Audit",
                entityId: null,
                details: new { Action = "QueryByEntityType", SearchEntityType = entityType, Page = page, PageSize = pageSize, Count = audits.Count },
                ct: ct);

            return Ok(new
            {
                Data = audits,
                Pagination = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }
    }
}
