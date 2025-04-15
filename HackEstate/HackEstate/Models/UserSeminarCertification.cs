using System;
using System.Collections.Generic;

namespace HackEstate.Models;

public partial class UserSeminarCertification
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public int? UserId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual User? User { get; set; }
}
