using System;
using System.Collections.Generic;

namespace HackEstate.Models;

public partial class AgentProperty
{
    public int Id { get; set; }

    public int? PropertyId { get; set; }

    public int? AgentId { get; set; }

    public virtual User? Agent { get; set; }

    public virtual Property? Property { get; set; }
}
