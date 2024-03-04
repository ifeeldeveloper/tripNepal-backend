using AdminPanelTN.DAL;
using AdminPanelTN.DTO;
using AdminPanelTN.Model;
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
    public class UserController : ControllerBase
    {
        private readonly TripNepalDbContext _dbContext;
        private static readonly List<string> _blacklist = new List<string>();
        private readonly IConfiguration _configuration;
        public UserController(TripNepalDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // POST api/authentication/signup
        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Invalid username or password");
            }

            if (_dbContext.Users.Any(u => u.UserName == user.UserName))
            {
                return Conflict("Username already exists");
            }

            User newUser = new User();
            newUser.UserName = user.UserName;
            newUser.Password = user.Password;

            _dbContext.Add(newUser);
            _dbContext.SaveChanges();

            return Ok("User created successfully");
        }

        [HttpPost]
        public IActionResult Post(int id, [FromBody] UserCredentialDTO userCredentialDTO)
        {
            var UserCredentials = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            if (userCredentialDTO.UserName == UserCredentials.UserName && userCredentialDTO.Password == UserCredentials.Password)
            {
                var authClaims = new List<Claim>
         {
             new Claim(ClaimTypes.Name, UserCredentials.UserName),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
         };
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
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
