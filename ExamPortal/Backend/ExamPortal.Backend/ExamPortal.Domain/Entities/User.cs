using System;
using System.Collections.Generic;

namespace ExamPortal.Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Role { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();

    public virtual Tenant Tenant { get; set; } = null!;
}
