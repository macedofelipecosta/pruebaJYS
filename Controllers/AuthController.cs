using DTOs;
using LogicaAplicacion.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_GestionDeSalas_Jaume_Sere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    [Tags("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILdapAuthenticationService _authService;

        public AuthController(ILdapAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request, CancellationToken ct)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Debe proporcionar usuario y contraseña." });

            try
            {
                var result = await _authService.AuthenticateAsync(request.Username, request.Password, ct);
                if (!result.Succeeded || result.User == null)
                {
                    return Unauthorized(new LoginResponseDTO
                    {
                        Success = false,
                        Message = "Credenciales inválidas."
                    });
                }

                var user = result.User;
                return Ok(new LoginResponseDTO
                {
                    Success = true,
                    Username = user.Username,
                    Email = user.Email.Value,
                    Role = user.Role,
                    LastLogin = user.LastLogin
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno al procesar la autenticación." });
            }
        }
    }
}
