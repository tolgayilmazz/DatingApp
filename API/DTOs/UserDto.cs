using System.Reflection.Metadata;

namespace API;

public class UserDto{
    public required string Username {get; set;}

    public required string Token {get;set;}

    public required string RefreshToken {get; set;}

    public required string Name {get; set;}

    public required string Role {get;set;}
}