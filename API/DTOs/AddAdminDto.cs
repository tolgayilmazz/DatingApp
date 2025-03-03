using System.Collections.Generic;
using System;

namespace API.DTOs;



public class AddAdminDto
{
    public int AdminId { get; set; }
    public List<int> ClubIds { get; set; } = new List<int>();
}
