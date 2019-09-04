using System.IO;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProAgil.API.Dtos;
using Microsoft.Extensions.Configuration;
using ProAgil.Domain.Identity;

namespace ProAgil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IConfiguration _config { get; }
        private UserManager<User> _userManager; 
        private SignInManager<User> _signInManager; 
        private IMapper _mapper;
        public UserController(IConfiguration config,
                            UserManager<User> userManager,
                            SignInManager<User> signInManager,
                            IMapper mapper)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            
            return Ok(new UserDTO());
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            try{
                var user = _mapper.Map<User>(userDTO);
                var result = await _userManager.CreateAsync(user, userDTO.Password);
                var userToReturn = _mapper.Map<UserDTO>(user);

                if(result.Succeeded)
                {
                    return Created("GetUser", userToReturn);
                }

                return BadRequest(result.Errors);

            }catch(System.Exception ex){
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO userLogin)
        {
            try{
                var user = await _userManager.FindByNameAsync(userLogin.UserName);

                var result = await _signInManager.CheckPasswordSignInAsync(user, userLogin.Password, false);

                if(result.Succeeded)
                {
                    var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userLogin.UserName.ToUpper());

                    var userToReturn = _mapper.Map<User, UserDTO>(appUser);

                    return Ok(new {
                        token = GenerateJwToken(appUser).Result,
                        UserName = userToReturn   
                    });
                }
            }catch(System.Exception){

            }

            return Unauthorized();
        }

        private async Task<string> GenerateJwToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach(var role in roles){
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}