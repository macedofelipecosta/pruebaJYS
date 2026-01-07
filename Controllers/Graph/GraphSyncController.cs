using Infrastructure.LogicaDatos.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize] // poné policy/rol admin si tenés
public class GraphSyncController : ControllerBase
{
    private readonly GestorSalasContext _db;

    public GraphSyncController(GestorSalasContext db) => _db = db;

    [HttpGet("runs")]
    public async Task<IActionResult> GetRuns([FromQuery] int top = 30, CancellationToken ct = default)
    {
        top = Math.Clamp(top, 1, 200);

        var runs = await _db.GraphSyncRuns
            .OrderByDescending(x => x.StartedAtUtc)
            .Take(top)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.InstanceId,
                x.StartedAtUtc,
                x.FinishedAtUtc,
                x.Status,
                x.Error,
                x.BuildingsFetched,
                x.BuildingsUpserted,
                x.BuildingsDeactivated,
                x.RoomsFetched,
                x.RoomsUpserted,
                x.RoomsDeactivated,
                x.UsersFetched,
                x.UsersUpserted,
                x.UsersDeactivated
            })
            .ToListAsync(ct);

        return Ok(runs);
    }
}
