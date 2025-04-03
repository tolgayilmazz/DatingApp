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
using Microsoft.AspNetCore.Authorization;


namespace API.Controllers;

public class ClubController : BaseApiController
{
    private readonly DataContext _context;

    public ClubController(DataContext context)
    {
        _context = context;
    }

    [HttpPost("add-club")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AddClub([FromBody] AddClubDto dto)
    {

        if (dto == null || string.IsNullOrWhiteSpace(dto.ClubName))
        {
            return BadRequest("Club name is required!");
        }

        var existingClub = await _context.Clubs.FirstOrDefaultAsync(c => c.ClubName == dto.ClubName);

        if (existingClub != null)
        {
            return BadRequest("Club already exists!");
        }

        var newClub = new Club
        {
            ClubName = dto.ClubName,
            LogoUrl = dto.LogoUrl,
            Description = dto.Description
        };

        await _context.Clubs.AddAsync(newClub);
        await _context.SaveChangesAsync();

        return Ok(newClub);

    }

    [HttpGet("get-clubs")]

    public async Task<ActionResult<List<Club>>> GetClubs()
    {
        var clubs = await _context.Clubs.ToListAsync();
        return Ok(clubs);
    }


    [HttpDelete("delete-club/{clubId}")]
    [Authorize(Roles = "SuperAdmin")]

    public async Task<IActionResult> DeleteClubs(int clubId)
    {
        var club = await _context.Clubs.FindAsync(clubId);

        if (club == null)
        {
            return NotFound("Club not found.");
        }

        var relatedAdminClubs = await _context.AdminClubs.Where(ac => ac.ClubId == clubId).ToListAsync();

        _context.AdminClubs.RemoveRange(relatedAdminClubs);

        var eventIdsToDelete = await _context.EventClubs
            .Where(ec => ec.ClubId == clubId)
            .Select(ec => ec.EventId)
            .Distinct()
            .ToListAsync();

        var eventTicketsToDelete = await _context.EventTickets
            .Where(et => eventIdsToDelete.Contains(et.EventId))
            .ToListAsync();

        var ticketIds = eventTicketsToDelete.Select(et => et.TicketId).Distinct().ToList();

        var ticketsToCheck = await _context.Tickets
            .Where(t => ticketIds.Contains(t.TicketId))
            .Include(t => t.EventTickets)
            .ToListAsync();

        _context.EventTickets.RemoveRange(eventTicketsToDelete);

        foreach (var ticket in ticketsToCheck)
        {
            if (ticket.EventTickets.Count == 1) 
            {
                _context.Tickets.Remove(ticket);
            }
        }
        
        var eventsToDelete = await _context.Events
            .Where(e => eventIdsToDelete.Contains(e.EventId))
            .ToListAsync();

        _context.Events.RemoveRange(eventsToDelete);

        var relatedEventClubs = await _context.EventClubs.Where(ec => ec.ClubId == clubId).ToListAsync();

        _context.EventClubs.RemoveRange(relatedEventClubs);


        _context.Clubs.Remove(club);

        await _context.SaveChangesAsync();



        return Ok(new { success = true, message = $"Club '{club.ClubName}' deleted successfully." });


    }



}