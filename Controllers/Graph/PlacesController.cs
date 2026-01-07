using DTOs.Graph;
using LogicaAplicacion.ServiceInterfaces.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlacesController : ControllerBase
{
    private readonly IGraphPlacesService _graphPlaces;

    public PlacesController(IGraphPlacesService graphPlaces)
    {
        _graphPlaces = graphPlaces;
    }

    [HttpGet("graph")]
    public async Task<ActionResult<IReadOnlyList<RoomDto>>> GetRoomsFromGraph(CancellationToken ct)
        => Ok(await _graphPlaces.ListRoomsAsync(ct));


    [HttpGet("buildings/graph")]
    public async Task<ActionResult<IReadOnlyList<BuildingDto>>> GetBuildingsFromGraph(CancellationToken ct)
       => Ok(await _graphPlaces.ListBuildingsAsync(ct));
}
