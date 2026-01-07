using DTOs.Graph;
using LogicaAplicacion.ServiceInterfaces.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IGraphUsersService _graphUsers;

    public UsersController(IGraphUsersService graphUsers)
    {
        _graphUsers = graphUsers;
    }

    [HttpGet("graph/search")]
    public async Task<ActionResult<IReadOnlyList<GraphUserDto>>> Search([FromQuery] string q, CancellationToken ct)
        => Ok(await _graphUsers.SearchUsersAsync(q, 25, ct));
}
