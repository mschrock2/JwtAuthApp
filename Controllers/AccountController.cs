using JwtAuthApp.Helpers;
using JwtAuthApp.Models;
using JwtAuthApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace JwtAuthApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("[action]"), ActionName("Create")]
        [ProducesResponseType<ApiResponse<AccountCreateResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse<AccountCreateResponse>>(StatusCodes.Status409Conflict)]
        [ProducesResponseType<ApiResponse<AccountCreateResponse>>(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] AccountRequest request)
        {
            ApiResponse<AccountCreateResponse> result = accountService.Create(request.username, request.password);
            switch (result.status)
            {
                case HttpStatusCode.OK:
                    return Ok(result);
                case HttpStatusCode.Conflict:
                    return Conflict(result);
                case HttpStatusCode.BadRequest:
                    return BadRequest(result);
                default:
                    return BadRequest(result);
            }
        }

        [HttpPost("[action]"), ActionName("Logon")]
        [ProducesResponseType<ApiResponse<AccountLogonResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse<AccountLogonResponse>>(StatusCodes.Status400BadRequest)]
        public IActionResult Logon([FromBody] AccountRequest accountRequest)
        {
            var result = accountService.Logon(accountRequest);
            switch (result.status)
            {
                case HttpStatusCode.OK:
                    return Ok(result);
                case HttpStatusCode.BadRequest:
                default:
                    return BadRequest(result);
            }
        }

        [HttpPost("[action]"), ActionName("SetStatus")]
        [ProducesResponseType<ApiResponse<SetStatusChange>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse<SetStatusChange>>(StatusCodes.Status400BadRequest)]
        public IActionResult SetStatus([FromBody] SetStatusChange setStatusRequest)
        {
            var result = accountService.SetStatus(setStatusRequest);
            switch (result.status)
            {
                case HttpStatusCode.OK:
                    return Ok(result);
                case HttpStatusCode.BadRequest:
                default:
                    return BadRequest(result);
            }
        }

        [HttpPost("[action]"), ActionName("Delete")]
        [ProducesResponseType<ApiResponse<SpecificUser>>(StatusCodes.Status200OK)]
        [ProducesResponseType<ApiResponse<SpecificUser>>(StatusCodes.Status400BadRequest)]
        public IActionResult Delete([FromBody] SpecificUser specificUser)
        {
            var result = accountService.Delete(specificUser);
            switch (result.status)
            {
                case HttpStatusCode.OK:
                    return Ok(result);
                case HttpStatusCode.BadRequest:
                default:
                    return BadRequest(result);
            }
        }

    }
}
