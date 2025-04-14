using System;
using System.Collections.Generic;

namespace HackEstate.Models;

public partial class Property
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? PropertyType { get; set; }

    public int? BathroomQty { get; set; }

    public int? BedroomQty { get; set; }

    public int? LotSize { get; set; }

    public double? Price { get; set; }

    public string? Location { get; set; }

    public string? Amenities { get; set; }

    public virtual ICollection<AgentProperty> AgentProperties { get; set; } = new List<AgentProperty>();

    public virtual ICollection<PropertyImage> PropertyImages { get; set; } = new List<PropertyImage>();

    public virtual User? User { get; set; }
}
