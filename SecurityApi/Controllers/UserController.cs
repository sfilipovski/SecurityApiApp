using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SecurityApi.Model.DTO;
using SecurityApi.Model.Models;
using SecurityApi.Repository.Interface;
using System.Threading.Tasks;

namespace SecurityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;

        public UserController(IConfiguration config, IUserRepository userRepository)
        {
            _config = config;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> GetByUsername(string username)
        {
            if(username == null) return BadRequest(string.Empty);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Username", username);

            var user = await _userRepository.ExecuteCommand<UserDto>("dbo.spUsers_GetByUsername", parameters, _config.GetConnectionString("Default"));

            if (user is null) return BadRequest("User doesn't exist");

            return Ok(user);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            if(id>0 && id < int.MaxValue)
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("UserId", id);

                var user = await _userRepository.ExecuteCommand<UserDto>("dbo.spUsers_GetById", parameters, _config.GetConnectionString("Default"));

                if (user is null) return BadRequest("User doesn't exist");

                return Ok(user);
            }

            return BadRequest("Invalid id value");
        }

        [HttpPost("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UserDto model)
        {
            if(ModelState.IsValid)
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("UserId", id);

                var user = await _userRepository.ExecuteCommand<UserDto>("dbo.spUsers_GetById", parameters, _config.GetConnectionString("Default"));

                if (user is null) return BadRequest($"User with id {id} doesn't exist");

                parameters.Add("Username", model.Username);
                parameters.Add("FirstName", model.FirstName);
                parameters.Add("LastName", model.LastName);

                var updatedUser = await _userRepository.ExecuteCommand<UserDto>("dbo.spUsers_Update", parameters, _config.GetConnectionString("Default"));

                return Ok(updatedUser);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if(id>0 && id < int.MaxValue)
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("UserId", id);

                var userExists = await _userRepository.ExecuteCommand<UserDto>("spUsers_GetById", parameters, _config.GetConnectionString("Default"));

                if (userExists is null) return BadRequest("User doesn't exist");

                
                await _userRepository.ExecuteCommand<UserDto>("spUsers_Delete", parameters, _config.GetConnectionString("Default"));

                return Ok();
            }

            return BadRequest("Invalid id value");
        }
    }
}
