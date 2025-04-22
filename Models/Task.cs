using System;
using System.Collections.Generic;

namespace CPMP.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public int? ProjectId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? CreatedBy { get; set; }

    public int? StatusId { get; set; }

    public DateOnly? DueDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual Project? Project { get; set; }

    public virtual TaskStatus? Status { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    public virtual ICollection<TimeLog> TimeLogs { get; set; } = new List<TimeLog>();
}
