using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApiAlex.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class LoginController : ControllerBase
    {
        /// <summary>
        /// Realiza login e gera um token JWT
        /// </summary>
        /// <param name="request">Credenciais do usuário</param>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request.User == "admin" && request.Password == "123")
                {
                    var key = Encoding.UTF8.GetBytes("gCK>|>Gz^r8e;yg8j;Z=d]J<P!vlWpx{GX*mL");
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, request.User),
                            new Claim(ClaimTypes.Role, "Administrador")
                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        Issuer = "WebApiAlex", // <-- Adicionado
                        Audience = "WebApiAlex", // <-- Adicionado
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    return Ok(new { token = tokenHandler.WriteToken(token) });
                }

                return Unauthorized(new ProblemDetails
                {
                    Title = "Credenciais inválidas",
                    Detail = "Usuário ou senha incorretos",
                    Status = StatusCodes.Status401Unauthorized
                });
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro ao realizar login",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }

    public class LoginRequest
    {
        public string User { get; set; }
        public string Password { get; set; }
    }
}
