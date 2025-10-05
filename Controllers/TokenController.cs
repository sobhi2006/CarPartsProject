using System.Security.Claims;
using CarPartsProject.DTOs.Requests.Auth;
using CarPartsProject.DTOs.Responses;
using CarPartsProject.Enums;
using CarPartsProject.Exceptions;
using CarPartsProject.Identity;
using CarPartsProject.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CarPartsProject.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiVersion("1.0")]
[Tags("Auth")]

public class AuthController(JwtTokenProvider jwtToken, IUserService userService) : ControllerBase
{
    private Guid? _currentUserId
        => Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!, out Guid Id)? Id : null;

    [HttpPost("login")]
    [MapToApiVersion("1.0")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("LoginAsync")]
    [EndpointSummary("Login user by email and password.")]
    [EndpointDescription("Login user by email and password and retrieve result.")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        var User = await userService.GetUserByEmailPasswordAsync(request.Email!, request.Password!);
        var token = jwtToken.GenerateToken(User);

        User.RefreshToken = token.RefreshToken;
        User.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(12);
        System.Console.WriteLine("before update user");
        await userService.UpdateUserAsync(User, _currentUserId);

        return Ok(new { t = token, Id = HttpContext.User.Identities });
    }

    [HttpPost("Refresh")]
    [MapToApiVersion("1.0")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("RefreshAsync")]
    [EndpointSummary("Refresh token for user when token has expired")]
    [EndpointDescription("Refresh token for user when token has expired, and returns refreshed result.")]
    public async Task<IActionResult> RefreshAsync(RefreshTokenRequest request)
    {
        var principal = jwtToken.GetClaimsPrincipalFromExpiredToken(request.AccessToken!)
                                ?? throw new BusinessRuleException("invalid token", StatusCodes.Status400BadRequest);

        var UserId = Guid.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!, out Guid Id)
                                ? Id : throw new BusinessRuleException("invalid token", StatusCodes.Status400BadRequest);

        var User = await userService.GetUserAsync(UserId);

        System.Console.WriteLine(User.RefreshToken + "  " + User.RefreshTokenExpiryTime);
        if (User == null
            || User.RefreshToken != request.RefreshToken
            || User.RefreshTokenExpiryTime < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token, please login");

        var token = jwtToken.GenerateToken(User);

        User.RefreshToken = token.RefreshToken;
        User.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(12);

        await userService.UpdateUserAsync(User, User.CreatedByUserId);

        return Ok(token);
    }

    [HttpPost("logout")]
    [Authorize(Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("LogoutAsync")]
    [EndpointSummary("Logout user from the system")]
    [EndpointDescription("Logout user from the system, which token not expired.")]
    public async Task<IActionResult> LogoutAsync()
    {
        var User = await userService.GetUserAsync((Guid)_currentUserId!);

        User.RefreshToken = null;
        User.RefreshTokenExpiryTime = DateTime.MinValue;
        System.Console.WriteLine("before update user");
        await userService.UpdateUserAsync(User, (Guid)_currentUserId!);

        return Ok("Logout successfully");
    }

    [HttpGet("me")]
    [Authorize(Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UserInfoAsync")]
    [EndpointSummary("Retrieve User Info for current user in the system")]
    [EndpointDescription("Retrieve User Info for current user in the system, and returns info result.")]
    public async Task<ActionResult<UserResponse>> UserInfoAsync()
    {
        throw new BusinessRuleException("koko", 9090);
        var User = await userService.GetUserAsync((Guid)_currentUserId!, UserRole.Admin);
        return Ok(User);
    }
}
