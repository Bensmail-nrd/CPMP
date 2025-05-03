using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CPMP.Models;

public partial class TeamMember
{
    [Display(Name ="Team")]
    public int TeamId { get; set; }
    [Display(Name = "User")]
    public int UserId { get; set; }
    [Display(Name ="Role in team")]
    public string? RoleInTeam { get; set; }

    public virtual Team? Team { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}

