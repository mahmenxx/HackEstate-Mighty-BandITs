using System;
using System.Collections.Generic;

namespace HackEstate.Models;

public partial class UserQuizAnswer
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? TypeOfProperty { get; set; }

    public string? PreferredLocation { get; set; }

    public int? BudgetMin { get; set; }

    public int? BudgetMax { get; set; }

    public string? WhenToBuy { get; set; }

    public string? PreferCommunication { get; set; }

    public string? MostImportantInAgent { get; set; }

    public virtual User? User { get; set; }
}
