using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CPMP.Models;

public partial class User
{
    public int UserId { get; set; }
    [Display(Name ="User name")]
    [Required(ErrorMessage = "User name is required")]
    [StringLength(50, ErrorMessage = "User name cannot be longer than 50 characters")]
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    public virtual ICollection<TimeLog> TimeLogs { get; set; } = new List<TimeLog>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
