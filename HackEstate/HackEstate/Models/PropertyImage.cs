using System;
using System.Collections.Generic;

namespace HackEstate.Models;

public partial class PropertyImage
{
    public int Id { get; set; }

    public int? PropertyId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Property? Property { get; set; }
}
