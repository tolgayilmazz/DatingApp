namespace API.DTOs
{
    public class UpdateUserRoleDto
    {
        public required int UserId { get; set; }
        public required string NewRole { get; set; }
    }
}