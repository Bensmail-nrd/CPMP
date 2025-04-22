using System;
using System.Collections.Generic;

namespace CPMP.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public int? ProjectId { get; set; }

    public string? Name { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
