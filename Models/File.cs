using System;
using System.Collections.Generic;

namespace CPMP.Models;

public partial class File
{
    public int FileId { get; set; }

    public int? TaskId { get; set; }

    public int? UploadedBy { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual Task? Task { get; set; }

    public virtual User? UploadedByNavigation { get; set; }
}
