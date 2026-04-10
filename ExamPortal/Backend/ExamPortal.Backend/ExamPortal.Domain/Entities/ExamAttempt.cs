using System;
using System.Collections.Generic;

namespace ExamPortal.Domain.Entities;

public partial class ExamAttempt
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ExamSetId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal? TotalScore { get; set; }

    public virtual ExamSet ExamSet { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
