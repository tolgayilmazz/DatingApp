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
        var result = await _adminService.AddAdminToClubs(dto);
        if (result.Contains("required") || result.Contains("not found"))
            return BadRequest(result);
        
        return Ok(result);
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
    public async Task<IActionResult> DeleteAdmin([FromBody] DeleteAdminDto dto)
    {
        var result = await _adminService.DeleteAdmin(dto);
        if (result.Contains("not found"))
            return BadRequest(result);

        return Ok(result);
    }
}