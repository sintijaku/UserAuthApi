using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Services;
using UserAPI.Dtos;
using UserAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private IAppUserService _service;
        private IConfiguration _config;

        public AppUserController(IAppUserService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AppUserRegisterDto appUserRegisterDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            appUserRegisterDto.UserName = appUserRegisterDto.UserName.ToLower();

            if (await _service.UserExsists(appUserRegisterDto.UserName))
            {
                return BadRequest("Username is already taken");
            }
            else
            {
                var userToCreate = new AppUser
                {
                    Username = appUserRegisterDto.UserName
                };

                var createUser = await _service.Register(userToCreate, appUserRegisterDto.Password);

            }
            return StatusCode(201);


        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AppUserRegisterDto appUserRegisterDto)
        {
            var appUser = await _service.Login(appUserRegisterDto.UserName.ToLower(), appUserRegisterDto.Password);
            if (appUser==null)
            {
                return Unauthorized();
            }

            //generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier,appUser.ID.ToString()),
                    new Claim(ClaimTypes.Name, appUser.Username)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { tokenString });
        }

    }
}