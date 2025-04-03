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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEvents(int id){
        var eventToDelete = await _context.Events
            .Include(e => e.EventClubs)
            .Include(e => e.EventTickets)
            .FirstOrDefaultAsync(e => e.EventId == id);
        if(eventToDelete == null) return NotFound("Event Not Found!");

        var relatedTicketIds = eventToDelete.EventTickets.Select(et => et.TicketId).ToList();

        var relatedTickets = await _context.Tickets
            .Where(t => relatedTicketIds.Contains(t.TicketId))
            .Include(t => t.EventTickets)
            .ToListAsync();
        
        _context.EventTickets.RemoveRange(eventToDelete.EventTickets);

        foreach (var ticket in relatedTickets)
        {
            if (ticket.EventTickets.Count == 1) 
            {
                _context.Tickets.Remove(ticket);
            }
        }

        _context.EventClubs.RemoveRange(eventToDelete.EventClubs);

        _context.Events.Remove(eventToDelete);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Event deleted successfully!" });
    } 


    [HttpGet("get-club-events/{clubId}")]
    [Authorize(Roles = "Admin")]
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
    
    [HttpPut("update-event/{eventId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateEvent(int eventId, [FromBody] EventDto UpdatedEvent){

        var existingEvent = await _context.Events.FindAsync(eventId);

        if(existingEvent == null) return NotFound("Event could not be found!");

        existingEvent.EventName = UpdatedEvent.EventName;
        existingEvent.EventDate = UpdatedEvent.EventDate;
        existingEvent.EventDescription = UpdatedEvent.EventDescription;
        existingEvent.Photo = UpdatedEvent.Photo;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Event updated successfully!" });
    }

    [HttpGet("get-an-event/{eventId}")]
    public async Task<IActionResult> GetAnEvent(int eventId){
        var events = await _context.Events
            .Where(e => e.EventId == eventId) 
            .Select(e => new
            {
                e.EventId,
                e.Photo,
                e.EventName,
                e.EventDescription,
                e.EventDate
            })
            .FirstOrDefaultAsync();

        
        if(events == null) return NotFound("Event Not Found!!");
        

        return Ok(events);
    }



}

