using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SecurityApi.Model.DTO;
using SecurityApi.Model.Models;
using SecurityApi.Repository.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace SecurityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;

        public AuthenticationController(IConfiguration config, IUserRepository userRepository)
        {
            _config = config;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserModel>> Register([FromBody] UserModel model)
        {
            if(ModelState.IsValid)
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Username", model.Username);

                var userExists = await _userRepository.ExecuteCommand<UserModel>("spUsers_GetByUsername", parameters, _config.GetConnectionString("Default"));

                if(userExists != null) {
                    return BadRequest($"The username {model.Username} is already taken");
                }

                string passwordHash = BC.HashPassword(model.PasswordHash);

                parameters.Add("FirstName", model.FirstName);
                parameters.Add("LastName", model.LastName);
                parameters.Add("Role", model.Role);
                parameters.Add("PasswordHash", passwordHash);

                var user = await _userRepository.ExecuteCommand<UserDto>("spUsers_Create", parameters, _config.GetConnectionString("Default"));

                if(user != null) {
                    return Ok(user);
                }

            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = ValidateCredentials(model);

                if(user.Result is null) {
                    return Unauthorized();
                }

                var token = GenerateToken(user.Result);

                return Ok(token);
            }

            return BadRequest(ModelState);
        }

        private async Task<UserModel> ValidateCredentials(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Username", model.Username);

                var user =  await _userRepository.ExecuteCommand<UserModel>("dbo.spUsers_GetByUsername", parameters, _config.GetConnectionString("Default"));

                if(user is null) {
                    return null;
                }

                bool passwordsMatch = BC.Verify(model.PasswordHash, user.PasswordHash);

                if (passwordsMatch)
                    return user;
            }
            return null;
        }

        private string GenerateToken(UserModel userModel)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.GetValue<string>("Authentication:SecretKey")));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, userModel.Username),
                new Claim(JwtRegisteredClaimNames.GivenName, userModel.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, userModel.LastName),
                new Claim(ClaimTypes.Role, userModel.Role)
            };

            var token = new JwtSecurityToken
            (
                _config.GetValue<string>("Authentication:Issuer"),
                _config.GetValue<string>("Authentication:Audience"),
                claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(1),
                signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
