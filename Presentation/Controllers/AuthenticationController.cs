using Entities.DataTransferObjects;
using Entities.DataTransferObjects.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
namespace Presentation.Controllers;

[ApiController]
[Route("api/authentication")]
[ApiExplorerSettings(GroupName = "v1")]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _services;

    public AuthenticationController(IServiceManager services)
    {
        _services = services;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUserAsync([FromBody] UserForRegistrationDto userForRegistrationDto)
    {
        IdentityResult result = await _services.AuthenticationService.RegisterUserAsync(userForRegistrationDto);

        if (!result.Succeeded)
        {
            foreach (IdentityError? error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        return StatusCode(201);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> AuthenticateAsync([FromBody] UserForAuthenticationDto userForAuthenticationDto)
    {
        if (!await _services.AuthenticationService.ValidateUserAsync(userForAuthenticationDto)) return Unauthorized();

        var tokenDto = await _services.AuthenticationService.CreateTokenAsync(populateExpire: true);
        return Ok(tokenDto);
    }

    [Authorize]
    [HttpPost("refresh")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenDto tokenDto)
    {
        TokenDto newTokenDto = await _services.AuthenticationService.RefreshTokenAsync(tokenDto);
        return Ok(newTokenDto);
    }
}
