using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminService = API.Services.AdminService;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;


namespace API.Controllers;


public class AdminController: BaseApiController{
    private readonly AdminService _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }
   

    [HttpGet("admins-with-users")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<List<AdminWithUserDto>>> GetAdminsWithUsers()
    {
        var result = await _adminService.GetAdminsWithUsers();
        return Ok(result);
    }


    [HttpGet("admins-with-clubs")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<List<AdminWithClubDto>>> GetAdminsWithClubs()
    {
        var result = await _adminService.GetAdminsWithClubs();
        return Ok(result);
    }

    [HttpGet("clubs-with-admins")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<List<ClubWithAdminDto>>> GetClubsWithAdmins()
    {
        var result = await _adminService.GetClubsWithAdmins();
        return Ok(result);
    }

    [HttpPost("add-admin-to-clubs")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AddAdminToClubs([FromBody] AddAdminDto dto)
    {
        Console.WriteLine($"Received AdminId: {dto.AdminId}, ClubIds: {JsonConvert.SerializeObject(dto.ClubIds)}");

        if (dto.ClubIds == null || !dto.ClubIds.Any())
        {
            Console.WriteLine("Error: ClubIds is null or empty!");
            return BadRequest("ClubIds cannot be empty.");
        }

        var result = await _adminService.AddAdminToClubs(dto);
        if (result.Contains("required") || result.Contains("not found")){
            Console.WriteLine($"Error: {result}");
            return BadRequest(new {error = result});
        }
        Console.WriteLine("Success");
        return Ok(new {success = true , message = "Admin succesfully assigned to the club." });
    }


    [HttpPut("update-admin-clubs")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateAdminClubs([FromBody] UpdateAdminDto dto)
    {
        var result = await _adminService.UpdateAdminClubs(dto);
        if (result.Contains("not found"))
            return BadRequest(result);
        
        return Ok(result);
    }
   

    [HttpDelete("delete-admin/{adminId}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteAdmin(int adminId)
    {
        var dto = new DeleteAdminDto{AdminId = adminId};
        var result = await _adminService.DeleteAdmin(dto);
        if (result.Contains("not found"))
            return NotFound(new {success = false, error = "Admin-club relation not found"});

        

        return Ok(new {success = true, message = "Admin-club relation succesfully deleted."});
    }
}