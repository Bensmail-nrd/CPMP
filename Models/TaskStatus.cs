﻿using System;
using System.Collections.Generic;

namespace CPMP.Models;

public partial class TaskStatus
{
    public int TaskStatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
