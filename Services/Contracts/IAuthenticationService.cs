using Entities.DataTransferObjects;
using Entities.DataTransferObjects.UserDtos;
using Microsoft.AspNetCore.Identity;

namespace Services.Contracts;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto);
    Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuthenticationDto);
    Task<TokenDto> CreateTokenAsync(bool populateExpire);
    Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto);
}
