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
using AddClubDto = API.DTOs.AddClubDto;


namespace API.Controllers;

public class ClubController : BaseApiController {
    private readonly DataContext _context;

    public ClubController(DataContext context)
    {
        _context = context;
    }

    [HttpPost("add-club")]
    public async Task<IActionResult> AddClub([FromBody] AddClubDto dto){

        if (dto == null || string.IsNullOrWhiteSpace(dto.ClubName)){
            return BadRequest("Club name is required!");
        }

        var existingClub = await _context.Clubs.FirstOrDefaultAsync(c => c.ClubName == dto.ClubName);

        if (existingClub != null){
            return BadRequest("Club already exists!");
        }

        var newClub = new Club{
            ClubName = dto.ClubName,
            LogoUrl = dto.LogoUrl,
            Description = dto.Description
        };

        await _context.Clubs.AddAsync(newClub);
        await _context.SaveChangesAsync();

        return Ok(newClub);

    }

    [HttpGet("get-clubs")]

    public async Task<ActionResult<List<Club>>> GetClubs(){
        var clubs = await _context.Clubs.ToListAsync();
        return Ok(clubs);
    } 

}