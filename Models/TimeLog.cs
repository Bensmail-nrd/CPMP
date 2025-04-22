using System;
using System.Collections.Generic;

namespace CPMP.Models;

public partial class TimeLog
{
    public int TimeLogId { get; set; }

    public int? TaskId { get; set; }

    public int? UserId { get; set; }

    public decimal? HoursWorked { get; set; }

    public DateOnly? LogDate { get; set; }

    public string? Notes { get; set; }

    public virtual Task? Task { get; set; }

    public virtual User? User { get; set; }
}
