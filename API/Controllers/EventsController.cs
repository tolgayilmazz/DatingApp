using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
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


public class EventsController : BaseApiController
{
    private readonly DataContext _context;

    public EventsController(DataContext context)
    {
        _context = context;
    }

    [HttpPost("create-events")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
    {
        try{
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(!int.TryParse(userId, out int userIdInt)){
            return Unauthorized("Invalid user Id!");
        }

        var admin = await _context.Admins.FirstOrDefaultAsync(a => a.UserID == userIdInt);
        if (admin == null)
        {
            return Unauthorized("You are not an admin!");
        }

        var adminClubIds = await _context.AdminClubs
            .Where(ac => ac.AdminId == admin.AdminId)
            .Select(ac => ac.ClubId)
            .ToListAsync();

        if (!eventDto.ClubIds.Any(cid => adminClubIds.Contains(cid)))
        {
            return Forbid("You are not allowed to create events in this club!");
        }

        var newEvent = new Event
        {
            Photo = eventDto.Photo,
            EventName = eventDto.EventName,
            EventDescription = eventDto.EventDescription,
            EventDate = DateTime.SpecifyKind(eventDto.EventDate, DateTimeKind.Utc)
        
        };

        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();

        var eventClubs = eventDto.ClubIds.Select(cid => new EventClub{
            EventId = newEvent.EventId,
            ClubId = cid
        }).ToList();

        _context.EventClubs.AddRange(eventClubs);
        await _context.SaveChangesAsync();

        var response = new
        {
            newEvent.EventId,
            newEvent.Photo,
            newEvent.EventName,
            newEvent.EventDescription,
            newEvent.EventDate
        };

        return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Database error!", error = ex.InnerException?.Message ?? ex.Message });
        }

    }

    [HttpGet("get-events")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetEvents(){
        var events = await _context.Events
            .Include(e => e.EventClubs)
            .ThenInclude(ec => ec.Club)
            .ToListAsync();
        
        

        var response = events.Select(e => new
        {
            e.EventId,
            e.Photo,
            e.EventName,
            e.EventDescription,
            e.EventDate,

            Clubs = e.EventClubs.Select(ec => new
            {
                ec.Club.ClubId,
                ec.Club.ClubName
            }).ToList()

        }).ToList();

        

        return Ok(response);
    }

    [HttpDelete("delete-events/{id}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> DeleteEvents(int id){
        var eventToDelete = await _context.Events
            .Include(e => e.EventClubs)
            .FirstOrDefaultAsync(e => e.EventId == id);
        if(eventToDelete == null) return NotFound("Event Not Found!");




        _context.EventClubs.RemoveRange(eventToDelete.EventClubs);

        _context.Events.Remove(eventToDelete);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Event deleted successfully!" });
    } 


    [HttpGet("get-club-events/{clubId}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetClubEvents(int clubId){
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if(!int.TryParse(userId, out int userIdInt)){
            return Unauthorized("Invalid user Id!!");
        }

        var admin = await _context.Admins.FirstOrDefaultAsync(a => a.UserID == userIdInt);

        if (admin == null){
            return Unauthorized("You are not an admin!");
        }

        var adminClubIds = await _context.AdminClubs
            .Where(ac => ac.AdminId == admin.AdminId)
            .Select(ac => ac.ClubId)
            .ToListAsync();

        if(!adminClubIds.Contains(clubId)) return Forbid("You are not assigned to this club!");

        var clubEvents = await _context.EventClubs
            .Where(ec => ec.ClubId == clubId)
            .Select(ec => ec.Event)
            .ToListAsync();

        var response = clubEvents.Select(e => new
        {
            e.EventId,
            e.Photo,
            e.EventName,
            e.EventDescription,
            e.EventDate
        }).ToList();

        return Ok(response);
    }
    



}

