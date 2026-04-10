using System;
using System.Collections.Generic;

namespace ExamPortal.Domain.Entities;

public partial class AnswerOption
{
    public Guid Id { get; set; }

    public Guid QuestionSetId { get; set; }

    public string OptionText { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public virtual QuestionSet QuestionSet { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
