using System.Collections.Generic;
using System.Security.Claims;
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
using System.Text.Json;


namespace API.Controllers;

public class TicketController : BaseApiController {

    private readonly DataContext _context;

    public TicketController(DataContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    public async Task<IActionResult> CreateTicket(CreateTicketDto dto){
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var ticket = new Ticket
        {
            UserId = userId,
            ReceiptPhoto = dto.ReceiptPhoto,
            Approved = false,
            Rejected = false
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        var EventTicket = new EventTicket
        {
            TicketId = ticket.TicketId,
            EventId = dto.EventId
        };

        _context.EventTickets.Add(EventTicket);
        _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Ticket request submitted successfully!" });
    }


    [HttpGet("my-tickets")]
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    public async Task<IActionResult> GetMyTickets() {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var MyTickets = await _context.Tickets
            .Where(t => t.UserId == userId)
            .Include(t => t.EventTickets)
                .ThenInclude(et => et.Event)
            .Select(t => new UserTicketDto
            {
                TicketId = t.TicketId,
                ReceiptPhoto = t.ReceiptPhoto,
                Approved = t.Approved,
                Rejected = t.Rejected,
                EventName = t.EventTickets.First().Event.EventName,
                EventDate = t.EventTickets.First().Event.EventDate
            })
            .ToListAsync();

        return Ok(MyTickets);
    }


    [HttpGet("event-buyers/{eventId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetClubBuyers(int eventId){
        var clubIdsClaim = User.FindFirst("ClubIds")?.Value;
        var allowedClubs = JsonSerializer.Deserialize<List<int>>(clubIdsClaim ?? "[]");

        var eventClubIds = await _context.EventClubs
            .Where(ec => ec.EventId == eventId)
            .Select(ec => ec.ClubId)
            .ToListAsync();

        bool isAuthorized = eventClubIds.Any(cid => allowedClubs.Contains(cid));
        if(!isAuthorized) return Forbid();

        var tickets = await _context.Tickets
            .Include(t => t.User)
            .Include(t => t.EventTickets)
                .ThenInclude(et => et.Event)
            .Where(t => t.EventTickets.Any(et => et.EventId == eventId))
            .Select(t => new ClubTicketDto
            {
                TicketId = t.TicketId,
                UserName = t.User.UserName,
                UserEmail = t.User.Email,
                ReceiptPhoto = t.ReceiptPhoto,
                Approved = t.Approved,
                Rejected = t.Rejected,
                EventName = t.EventTickets.First().Event.EventName,
                EventDate = t.EventTickets.First().Event.EventDate
            })
            .ToListAsync();

        return Ok(tickets);
    }


    [HttpPut("approve/{ticketId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveTicket(int ticketId) {
        var ticket = await _context.Tickets.FindAsync(ticketId);

        if(ticket == null) return NotFound("Ticket Not Found!!");

        if (ticket.Rejected) return BadRequest("Cannot approve a rejected ticket.");

        ticket.Approved = true;

        await _context.SaveChangesAsync();

            
        return Ok(new { success = true, message = "Ticket approved successfully!" });
    }

    [HttpPut("reject/{ticketId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectTicket(int ticketId){
        var ticket = await _context.Tickets.FindAsync(ticketId);

        if(ticket == null) return NotFound("Ticket Not Found!!");

        if(ticket.Approved) return BadRequest("Cannot reject an approved ticket.");

        ticket.Rejected = true;

        await _context.SaveChangesAsync();

        return Ok(new{success = true, message = "Ticket rejected succesfully!"});
    }

    [HttpPut("reset/{ticketId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetTicket(int ticketId){
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null)
            return NotFound("Ticket not found.");

        if (!ticket.Approved && !ticket.Rejected)
            return BadRequest("Ticket is already pending.");

        ticket.Approved = false;
        ticket.Rejected = false;

        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Ticket status reset to pending." });
    }

    [HttpDelete("delete/{ticketId}")]
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteTicket(int ticketId){
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var ticket = await _context.Tickets
            .Include(t => t.EventTickets)
            .FirstOrDefaultAsync(t => t.TicketId == ticketId);

        if (ticket == null)
            return NotFound("Ticket not found.");

        if(ticket.UserId != userId) return Forbid("You can only delete your own ticket.");

        if(ticket.Approved) return BadRequest("Approved tickets cannot be deleted.");

        _context.EventTickets.RemoveRange(ticket.EventTickets);

        _context.Tickets.Remove(ticket);

        await _context.SaveChangesAsync();

        return Ok(new {success = true, message = "Ticket deleted succesfully!"});
    }

} 