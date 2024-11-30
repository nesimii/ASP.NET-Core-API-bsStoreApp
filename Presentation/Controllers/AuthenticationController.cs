using Entities.DataTransferObjects.UserDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
namespace Presentation.Controllers;

[ApiController]
[Route("api/{v:apiversion}/authentication")]
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
}
