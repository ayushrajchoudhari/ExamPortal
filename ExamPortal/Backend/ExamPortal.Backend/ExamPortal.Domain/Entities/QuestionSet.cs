using System;
using System.Collections.Generic;

namespace ExamPortal.Domain.Entities;

public partial class QuestionSet
{
    public Guid Id { get; set; }

    public Guid ExamSetId { get; set; }

    public string QuestionText { get; set; } = null!;

    public decimal Points { get; set; }

    public int DisplayOrder { get; set; }

    public virtual ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();

    public virtual ExamSet ExamSet { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
