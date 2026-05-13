using System;
using System.Collections.Generic;

namespace ExamPortal.Domain.Entities;

public partial class ExamSet
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = null!;
    public int DurationMinutes { get; set; }
    public decimal PassingScore { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedByUserId { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? SecretToken { get; set; }
    public virtual InstructionSet? InstructionSet { get; set; }
    public virtual ICollection<QuestionSet> QuestionSets { get; set; } = new List<QuestionSet>();
    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();
    public virtual Tenant Tenant { get; set; } = null!;
}
