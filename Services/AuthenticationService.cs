using AutoMapper;
using Entities.DataTransferObjects;
using Entities.DataTransferObjects.UserDtos;
using Entities.Exceptions;
using Entities.Models;
using Enums.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILoggerService _loggerService;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IConfigurationSection _jwtSettings;

    private User? _user;

    public AuthenticationService(ILoggerService loggerService, IMapper mapper, UserManager<User> userManager, IConfiguration configuration)
    {
        _loggerService = loggerService;
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
        _jwtSettings = _configuration.GetSection("JwtSettings");
    }

    public async Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto)
    {
        User user = _mapper.Map<User>(userForRegistrationDto);

        IdentityResult result = await _userManager.CreateAsync(user, userForRegistrationDto.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, userForRegistrationDto.Roles);

            if (userForRegistrationDto.Claims != null)
            {
                IEnumerable<Claim> userClaims = userForRegistrationDto.Claims
                    .Where(claim => PermissionClaims.claimList.Contains(claim))
                    .Select(claim => new Claim(PermissionClaims.Permission, claim));
                if (userClaims.Any())
                {
                    await _userManager.AddClaimsAsync(user, userClaims);
                }
            }
        }
        return result;
    }

    public async Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuthenticationDto)
    {
        _user = await _userManager.FindByNameAsync(userForAuthenticationDto.UserName);
        bool result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuthenticationDto.Password));
        if (!result)
        {
            _loggerService.LogWarning($"{nameof(ValidateUserAsync)}: Authentication failed. Wrong username or password.");
        }
        return result;
    }

    public async Task<TokenDto> CreateTokenAsync(bool populateExpire)
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaimsAsync();
        JwtSecurityToken tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        string refreshToken = GenerateRefreshToken();
        _user!.RefreshToken = refreshToken;

        if (populateExpire)
        {
            _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        }
        await _userManager.UpdateAsync(_user);

        string accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] key = Encoding.UTF8.GetBytes(_jwtSettings["secretKey"]);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaimsAsync()
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, _user.UserName)
        };
        IList<string> roles = await _userManager.GetRolesAsync(_user);

        foreach (string? role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signInCredentials, List<Claim> claims)
    {
        IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");

        JwtSecurityToken tokenOptions = new JwtSecurityToken(
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expires"])),
            signingCredentials: signInCredentials
            );

        return tokenOptions;
    }

    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["secretKey"];

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["validIssuer"],
            ValidAudience = jwtSettings["validAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
            out securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken is null ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token.");
        }
        return principal;
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto tokenDto)
    {
        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        User user = await _userManager.FindByNameAsync(principal.Identity.Name);

        if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new RefreshTokenBadRequestException();
        }

        _user = user;
        return await CreateTokenAsync(populateExpire: false);
    }
}
