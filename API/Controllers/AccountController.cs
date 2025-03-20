

using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Services;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{


    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> RegisterUser(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username))
        {
            return BadRequest("Username already exists!");
        }
        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key,
            Name = registerDto.Name,
            Surname = registerDto.Surname,
            Email = registerDto.Email,
            Role = registerDto.Role ?? "User",
            RefreshToken = tokenService.CreateRefreshToken(),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(1)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            RefreshToken = user.RefreshToken,
            Name = user.Name,
            Role = user.Role
        };
    }

    [HttpPost("login")]

    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        user.RefreshToken = tokenService.CreateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);

        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            RefreshToken = user.RefreshToken,
            Name = user.Name,
            Role = user.Role
        };
    }

    [HttpPost("refresh-token")]

    public async Task<ActionResult<UserDto>> RefreshToken([FromBody] TokenRefreshDto tokenRefreshDto)
    {

        if (tokenRefreshDto == null || string.IsNullOrEmpty(tokenRefreshDto.RefreshToken))
        {
            return BadRequest("The refresh token cannot be empty!");
        }


        var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == tokenRefreshDto.RefreshToken);

        if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
        {
            return Unauthorized("Invalid or expired refresh token");
        }


        var newAccessToken = await tokenService.CreateToken(user);

        if (user.RefreshTokenExpiry.Subtract(DateTime.UtcNow).TotalMinutes < 30)
        {
            var newRefreshToken = tokenService.CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
        }

        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = newAccessToken,
            RefreshToken = user.RefreshToken,
            Name = user.Name,
            Role = user.Role
        };
    }

    [HttpPut("update-role")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto dto, [FromServices] AdminService adminService)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);

        if (user == null) return BadRequest("User not found!");

        if (user.Role == "SuperAdmin") return BadRequest("Cannot change the role of a SuperAdmin!");

        if (user.Role == dto.NewRole) return BadRequest("User already has this role!");

        user.Role = dto.NewRole;
        await context.SaveChangesAsync();

        if (dto.NewRole == "Admin")
        {
            try
            {
                await adminService.EnsureAdminExists(dto.UserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while trying to create an Admin record.");
            }

        }
        else
        {
            var existingAdmin = await context.Admins.FirstOrDefaultAsync(a => a.UserID == dto.UserId);
            if (existingAdmin != null)
            {
                context.Admins.Remove(existingAdmin);
                await context.SaveChangesAsync();
            }
        }

        var loggedInUserClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(loggedInUserClaim, out int loggedInUserId))
        {
            Console.WriteLine($"Error: Could not parse logged-in user ID. Claim value: {loggedInUserClaim}");
            return Unauthorized("Invalid user ID in token.");
        }

        if (user.Id == loggedInUserId)
        {
            var newAccessToken = await tokenService.CreateToken(user);
            return Ok(new
            {
                newAccessToken,
                message = "Role updated successfully!"
            });
        }

        return Ok(new { message = "Role updated successfully!" });
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }


}