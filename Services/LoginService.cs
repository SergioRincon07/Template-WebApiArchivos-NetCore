using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiArchivos.DataBaseArchivos.Context;
using WebApiArchivos.DataBaseArchivos.Models;
using WebApiArchivos.Models;

namespace WebApiArchivos.Services
{
    public class LoginService
    {
        private readonly ArchivosContext _context;

        public LoginService(ArchivosContext context)
        {
            _context = context;

        }
        public async Task<List<spLoginAlmacenamientoResult>> LoginAppMegaLinea(UsuarioLogin usuarioLogin)
        {
            List<spLoginAlmacenamientoResult> UserLogin = await _context.GetProcedures().spLoginAlmacenamientoAsync(usuarioLogin.Username, usuarioLogin.Password);
            return UserLogin;
        }
        public static string GenerateToken(IConfiguration configuration)
        {
            var jwt = configuration.GetSection("Jwt").Get<Jwt>();

            var now = DateTime.UtcNow;
            var expiresToken = DateTime.UtcNow.AddMinutes(10);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("startToken", now.ToString("yyyy-MM-dd HH:mm:ss")),
                new Claim("expiresToken", expiresToken.ToString("yyyy-MM-dd HH:mm:ss")),
                new Claim("username", "username")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(jwt.Issuer, jwt.Audience, claims, expires: expiresToken, signingCredentials: signing);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }


    }
}
