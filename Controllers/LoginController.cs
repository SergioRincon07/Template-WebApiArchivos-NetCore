using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using WebApiArchivos.DataBaseArchivos.Context;
using WebApiArchivos.Models;
using WebApiArchivos.Services;

namespace WebApiFieldBots.Controllers
{
    public class LoginController : Controller
    {
        private readonly ArchivosContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(ArchivosContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene el Token para el acceso de las APIs. 
        /// </summary>
        /// <remarks>
        /// Se hace uso del servicio "LoginService", el cual devuelve un Token si el usuario tiene Acceso, 
        /// el procedimiento almacenado para validar los accesos es "spLoginAlmacenamiento".
        /// En la base de datos Archivos hay una tabla llamada Almacenamiento en el cual se encuentran
        /// las aplicaciones que debertian tener accesso. 
        /// Ejemplo: USR->ProveedoresStorage: PSS->ProveedoresStorage, USR->CandidatosStorage: PSS->CandidatosStorage
        /// </remarks>
        [HttpPost]
        [Route("/Login")]
        [ProducesResponseType(typeof(Token), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<Token>> IniciarSesionAsync([FromBody] UsuarioLogin usuarioLogin)
        {
            try
            {
                if (usuarioLogin == null)
                {
                    return BadRequest("Datos incorrectos");
                }
                var LoginService = new LoginService(_context);
                var listUserLogin = await LoginService.LoginAppMegaLinea(usuarioLogin);

                if (listUserLogin.Count > 0)
                {
                    var token = LoginService.GenerateToken(_configuration);

                    var responseToken = new Token
                    {
                        TokenJWT = token,
                        Acceso = true
                    };

                    return Ok(responseToken);
                }
                else
                {
                    var responseToken = new Token
                    {
                        TokenJWT = "",
                        Acceso = false
                    };

                    return Ok(responseToken);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }

        }
    }
}
