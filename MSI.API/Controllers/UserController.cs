using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MSI.API.Models.DataContext;
using MSI.API.Models.Entity;
using MSI.API.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MSI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUserModel> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private MSIDataContext _context;

        public UserController(UserManager<ApplicationUserModel> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, MSIDataContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            this._context = context;
        }
        [HttpPost]
        [Route("login")]
       
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim("name", user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(30),
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

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status406NotAcceptable, new ResponseModel { Status = "Invalid Data", Message = "User already exists!" });

            var isMailExist = await userManager.FindByEmailAsync(model.Email);
            if(isMailExist!=null)
                return StatusCode(StatusCodes.Status406NotAcceptable, new ResponseModel { Status = "Invalid Data", Message = "Email already exists!" });

            string IDPhoneNo = $"62{model.PhoneNumber}";
            var isPhoneNoExist = _context.Users.FirstOrDefault(x => x.PhoneNumber == IDPhoneNo);
           
            if(isPhoneNoExist!=null)
                return StatusCode(StatusCodes.Status406NotAcceptable, new ResponseModel { Status = "Invalid Data", Message = "Phone Number already exists!" });

            ApplicationUserModel user = new ApplicationUserModel()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email,
                FirstName=model.FirstName,
                LastName=model.LastName,
                PhoneNumber=$"62{model.PhoneNumber}",
                Gender=model.Gender,
                DOB=model.DOB==null?DateTime.Parse("1900-01-01"):DateTime.Parse(model.DOB)
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        }

        [Authorize]
        [HttpGet]
        [Route("GetDataUser")]
        public async Task<IActionResult> GetDataUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            string name = string.Empty;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                 name = claims.Where(p => p.Type == "name").FirstOrDefault()?.Value;
                var detail = await userManager.FindByNameAsync(name);


                return Ok(new
                {
                    FirstName = detail == null ? string.Empty : detail.FirstName,
                    LastName = detail == null ? string.Empty : detail.LastName,
                });
            }
            else
            {
                return Unauthorized();
            }
               
        }

        [HttpGet]
        [Route("TestAPI")]
        public bool TestAPI()
        {
            return _context.Database.CanConnect();
        }


    }
}
