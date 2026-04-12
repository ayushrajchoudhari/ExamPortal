using System;
using System.Collections.Generic;

namespace ExamPortal.Domain.Entities;

public partial class Tenant
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Domain { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ExamSet> ExamSets { get; set; } = new List<ExamSet>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
