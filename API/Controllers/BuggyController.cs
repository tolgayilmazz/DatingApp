namespace API;
using API.Data;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Entities;

public class BuggyController(DataContext context) : BaseApiController{
    
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth(){
        return "secret text.";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound(){

        var thing = context.Users.Find(-1);
        if(thing == null) return NotFound();
        return thing;
    }

    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError(){
        var thing = context.Users.Find(-1) ?? throw new Exception("brooooo...");
        
        return thing;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest(){

        return BadRequest("This was not a good request");
    }
}