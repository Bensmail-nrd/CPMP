using CPMP.Helprs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CPMP.Models;

public partial class Task
{
    public int TaskId { get; set; }
    [Required]
    [Display(Name ="Project")]
    public int? ProjectId { get; set; }
    [Required]
    public string? Title { get; set; }
    [Required]
    [StringLength(maximumLength:50,MinimumLength =10, ErrorMessage ="The description should be aleast 10 caracters")]
    public string? Description { get; set; }
    [Display(Name ="Created By")]
    public int? CreatedBy { get; set; }
    [Display(Name ="Status")]
    public int? StatusId { get; set; }
    [Required]
    [Display(Name ="Due Date")]
    [FutureDate]
    public DateOnly? DueDate { get; set; }
    [Display(Name = "Created At")]
    public DateTime? CreatedAt { get; set; }
    [Display(Name = "Created By")]
    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual Project? Project { get; set; }

    public virtual TaskStatus? Status { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    public virtual ICollection<TimeLog> TimeLogs { get; set; } = new List<TimeLog>();
}
