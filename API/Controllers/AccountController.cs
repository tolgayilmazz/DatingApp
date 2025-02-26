

using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService): BaseApiController {
    [HttpPost("register-admin")] //acount/register
    [Authorize(Roles = "SuperAdmin")]

    public async Task<ActionResult<UserDto>> RegisterAdmin(RegisterDto registerDto){

        if (await UserExists(registerDto.Username)) return BadRequest("Username already exists!");

        if(registerDto.Role != "Admin" && registerDto.Role != "User" && registerDto.Role != "SuperAdmin") return BadRequest("Invalid role!");
        
        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key,
            Name = registerDto.Name,
            Surname = registerDto.Surname,
            Email = registerDto.Email,
            Role = registerDto.Role,
            RefreshToken = tokenService.CreateRefreshToken(),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(1)
            
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto{
            Username  = user.UserName,
            Token = tokenService.CreateToken(user),
            RefreshToken = user.RefreshToken,
            Name = user.Name,
            Role = user.Role
        };
    }

    [HttpPost("register-user")]
    public async Task<ActionResult<UserDto>> RegisterUser(RegisterDto registerDto){
        if (await UserExists(registerDto.Username)){
            return BadRequest("Username already exists!");
        }
        using var hmac = new HMACSHA512();

        var user = new AppUser{
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key,
            Name = registerDto.Name,
            Surname = registerDto.Surname,
            Email = registerDto.Email,
            Role = "User", 
            RefreshToken = tokenService.CreateRefreshToken(),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(1)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto{
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            RefreshToken = user.RefreshToken,
            Name = user.Name,
            Role = user.Role
        };
    }

    [HttpPost("login")]

    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

        if (user == null ) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++){
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        user.RefreshToken = tokenService.CreateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);

        await context.SaveChangesAsync();

        return new UserDto{
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            RefreshToken = user.RefreshToken,
            Name = user.Name,
            Role = user.Role
        };
    }

    [HttpPost("refresh-token")]

    public async Task<ActionResult<UserDto>> RefreshToken([FromBody] TokenRefreshDto tokenRefreshDto){

        if(tokenRefreshDto == null || string.IsNullOrEmpty(tokenRefreshDto.RefreshToken)){
            return BadRequest("The refresh token cannot be empty!");
        }


        var user = await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == tokenRefreshDto.RefreshToken);

        if(user == null || user.RefreshTokenExpiry <= DateTime.UtcNow ){
            return Unauthorized("Invalid or expired refresh token");
        }
        

        var newAccessToken = tokenService.CreateToken(user);
        var newRefreshToken = tokenService.CreateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);

        await context.SaveChangesAsync();

        return new UserDto{
            Username = user.UserName,
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            Name = user.Name,
            Role = user.Role
        };
    }

    private async Task<bool> UserExists(string username){
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }

   
}