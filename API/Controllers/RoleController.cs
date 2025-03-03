using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;



namespace API.Controllers
{
    public class RoleController: BaseApiController {
        [Authorize(Policy = "RequireSuperAdminRole")]
        [HttpGet("superadmin/manage-admins")]
        public IActionResult ManageAdmins(){
            return Ok("Access granted to SuperAdmin.");
        }
    
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("admin/club-info")]
        public IActionResult GetClubInfo(){
            var user = HttpContext.User;
            var username = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var ClubId = user.FindFirst("ClubId")?.Value;

            Console.WriteLine($"Authenticated User: {username}");
            Console.WriteLine($"User role: {role}");
            Console.WriteLine($"User Club Id: {ClubId}");

            if(role == "SuperAdmin"){
                return Ok("Access granted to SuperAdmin for all clubs.");
            }
            if(role == "Admin" && !string.IsNullOrEmpty(ClubId)){
                return Ok($"Access granted to Admin. Club Id: {ClubId}");
            }
            return Forbid("Access denied.");
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpGet("user/dashboard")]
        public IActionResult GetUserDashboard(){
            return Ok("Access granted to User.");
        }
    }

}