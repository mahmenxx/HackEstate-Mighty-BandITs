using System;
using System.Collections.Generic;

namespace HackEstate.Models;

public partial class Event
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Location { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public virtual User? User { get; set; }
}
