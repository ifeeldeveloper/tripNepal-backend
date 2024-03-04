using AdminPanelTN.DAL;
using AdminPanelTN.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminPanelTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthenticationController : ControllerBase
    {
        private readonly TripNepalDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public AuthenticationController(TripNepalDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post([FromBody] AdminCredentialDTO adminCredentialDTO)
        {
            var UserName = _configuration["AdminCredential:username"];
            var Password = _configuration["AdminCredential:password"];

            var issuer2 = _configuration.GetSection("JwtSettings:Issuer2");
            var validIssuer = issuer2["ValidIssuer"];
            var validAudience = issuer2["ValidAudience"];
            var secret = issuer2["Secret"];

            if (adminCredentialDTO.UserName == UserName && adminCredentialDTO.Password == Password)
            {
                var authClaims = new List<Claim>
         {
             new Claim(ClaimTypes.Name, UserName),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
         };
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

                var token = new JwtSecurityToken(
                    issuer: _configuration[validIssuer],
                    audience: _configuration[validAudience],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }
}
