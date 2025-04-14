using System;
using System.Collections.Generic;

namespace HackEstate.Models;

public partial class ChatMessage
{
    public int Id { get; set; }

    public int? FromUserId { get; set; }

    public int? ToUserId { get; set; }

    public string? Message { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual User? ToUser { get; set; }
}
