namespace API.Entities;


public class AppUser{

    public int Id { get; set; }

    public required string UserName { get; set; } 

    public required byte[] PasswordHash {get; set;}

    public required byte[] PasswordSalt {get; set;}

    public required string Name {get; set;}

    public required string Surname {get; set;}

    public required string Email {get; set;}

    public required string RefreshToken {get; set;}

    public required DateTime RefreshTokenExpiry {get; set;}

    public required string Role {get;set;} = "User";
}
