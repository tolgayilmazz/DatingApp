using System.ComponentModel.DataAnnotations;

namespace API;

public class RegisterDto{
    [Required]

    public required string Username {get; set;}

    [Required]
    public required string Password {get; set;}

    [Required]

    public required string Name {get; set;}

    [Required]

    public required string Surname {get; set;}

    [Required]
    [EmailAddress]
    public required string Email {get; set;}

    [Required]

    public required string Role {get;set;} = "User";

}