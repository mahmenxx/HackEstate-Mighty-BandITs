using System;
using System.Collections.Generic;

namespace HackEstate.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public int? RoleId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Address { get; set; }

    public string? Contact { get; set; }

    public string? Password { get; set; }

    public string? IdentificationCardUrl { get; set; }

    public virtual ICollection<AgentProperty> AgentProperties { get; set; } = new List<AgentProperty>();

    public virtual ICollection<ChatMessage> ChatMessageFromUsers { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ChatMessage> ChatMessageToUsers { get; set; } = new List<ChatMessage>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual Role? Role { get; set; }
}
