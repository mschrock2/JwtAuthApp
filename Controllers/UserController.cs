using JwtAuthApp.Authorization;
using JwtAuthApp.Helpers;
using JwtAuthApp.Models;
using JwtAuthApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Linq;

namespace JwtAuthApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [Auth("User:r")]
        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType<ApiResponse<User>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse<User>>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ApiResponse<User>>(StatusCodes.Status500InternalServerError)]
        public IActionResult Get(string id)
        {
            var result = userService.GetById(id);
            switch (result.status)
            {
                case System.Net.HttpStatusCode.OK:
                    
                        return Ok(result);
                    
                default:
                    return BadRequest(new ApiResponse<User> { error = result.error, status = System.Net.HttpStatusCode.BadRequest });
            }
        }

        [Auth("User:r")]
        [HttpGet]
        [ProducesResponseType<ApiResponse<IEnumerable<User>>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse<IEnumerable<User>>>(StatusCodes.Status500InternalServerError)]
        public IActionResult Get(int skip = 0, int take = 20, string? search = null)
        {
            var result = userService.Get(skip, take, search);
            switch (result.status)
            {
                case System.Net.HttpStatusCode.OK:
                    return Ok(result);
                default:
                    return BadRequest(result);
            }
        }

    }
}
