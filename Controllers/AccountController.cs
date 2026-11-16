using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMarkets.Data;
using StockMarkets.DTOs;
using StockMarkets.Exceptions;
using StockMarkets.Interfaces;
using StockMarkets.Models;

namespace StockMarkets.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService,
        SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _logger = logger;
    }

    // Invalid models are rejected by [ApiController] before reaching here, and unexpected
    // exceptions are handled by GlobalExceptionHandler, so no try/catch is needed.
    [HttpPost("register")]
    [ProducesResponseType(typeof(NewUserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerRequest)
    {
        var appUser = new AppUser { UserName = registerRequest.Username, Email = registerRequest.Email };

        var createdUser = await _userManager.CreateAsync(appUser, registerRequest.Password);
        if (!createdUser.Succeeded)
            throw ToDomainException(createdUser.Errors);

        var roleResult = await _userManager.AddToRoleAsync(appUser, RoleConstants.UserName);
        if (!roleResult.Succeeded)
        {
            // Do not leave a user without a role behind.
            await _userManager.DeleteAsync(appUser);
            // Log only Identity error codes (no user data); the client gets a generic 500.
            _logger.LogError("Role assignment failed during registration: {ErrorCodes}",
                string.Join(",", roleResult.Errors.Select(e => e.Code)));
            throw new InvalidOperationException("Role assignment failed during registration.");
        }

        var dto = new NewUserDto
        {
            UserName = appUser.UserName,
            Email = appUser.Email,
            Token = _tokenService.CreateToken(appUser, await _userManager.GetRolesAsync(appUser))
        };
        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginRequest)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginRequest.Username);
        if (user == null)
            return Unauthorized(new { Message = "Invalid username." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
        if (!result.Succeeded) return Unauthorized(new { Message = "Invalid password." });

        return Ok(
            new NewUserDto
            {
                UserName = user.UserName, Email = user.Email,
                Token = _tokenService.CreateToken(user, await _userManager.GetRolesAsync(user))
            });
    }

    /// <summary>Translates Identity's error list into a 409 (duplicates) or a field-level 400.</summary>
    private static DomainException ToDomainException(IEnumerable<IdentityError> identityErrors)
    {
        var errors = identityErrors.ToList();

        if (errors.Any(e => e.Code == nameof(IdentityErrorDescriber.DuplicateUserName)))
            return new ConflictException(ErrorCodes.UserExists, "Username is already taken.");

        if (errors.Any(e => e.Code == nameof(IdentityErrorDescriber.DuplicateEmail)))
            return new ConflictException(ErrorCodes.UserExists, "Email is already registered.");

        var byField = errors
            .GroupBy(e => FieldFor(e.Code))
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
        return new ValidationFailedException(byField);
    }

    private static string FieldFor(string identityErrorCode) => identityErrorCode switch
    {
        var c when c.StartsWith("Password") => "password",
        nameof(IdentityErrorDescriber.InvalidUserName) => "username",
        nameof(IdentityErrorDescriber.InvalidEmail) => "email",
        _ => "request"
    };
}
