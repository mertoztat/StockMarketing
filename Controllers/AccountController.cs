using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMarkets.Data;
using StockMarkets.DTOs;
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

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService,
        SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerRequest)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var appUser = new AppUser { UserName = registerRequest.Username, Email = registerRequest.Email };

            var createdUser = await _userManager.CreateAsync(appUser, registerRequest.Password);
            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(appUser, RoleConstants.UserName);
                if (roleResult.Succeeded)
                {
                    return Ok(
                        new NewUserDto
                        {
                            UserName = appUser.UserName,
                            Email = appUser.Email,
                            Token = _tokenService.CreateToken(appUser, await _userManager.GetRolesAsync(appUser))
                        });
                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, roleResult.Errors);
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, createdUser.Errors);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An error occurred during registration.", Error = e.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginRequest)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
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
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An error occurred during login.", Error = e.Message });
        }
    }
}
