using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Linq;
using System.Text.Json;



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
            var ClubIdsClaim = user.FindFirst("ClubIds")?.Value;


            var clubIds = new List<int>();
            if(!string.IsNullOrWhiteSpace(ClubIdsClaim)){
                try{
                    clubIds = JsonSerializer.Deserialize<List<int>>(ClubIdsClaim) ?? new List<int>();
                }
                catch(JsonException){
                    Console.WriteLine("Error parsing ClubIds JSON from token.");
                }
            }
            Console.WriteLine($"Authenticated User: {username}");
            Console.WriteLine($"User role: {role}");
            Console.WriteLine($"User Club Ids: {string.Join(", ", clubIds)}");

            if(role == "SuperAdmin"){
                return Ok("Access granted to SuperAdmin for all clubs.");
            }
            if(role == "Admin" && clubIds.Any()){
                return Ok($"Access granted to Admin. Club Ids: {string.Join(", ", clubIds)}");
            }
            return Forbid();
        }

        [Authorize(Policy = "RequireUserRole")]
        [HttpGet("user/dashboard")]
        public IActionResult GetUserDashboard(){
            return Ok("Access granted to User.");
        }
    }

}