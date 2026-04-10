using System;
using System.Collections.Generic;

namespace ExamPortal.Domain.Entities;

public partial class UserAnswer
{
    public Guid Id { get; set; }

    public Guid AttemptId { get; set; }

    public Guid QuestionId { get; set; }

    public Guid SelectedOptionId { get; set; }

    public bool IsCorrect { get; set; }

    public virtual ExamAttempt Attempt { get; set; } = null!;

    public virtual QuestionSet Question { get; set; } = null!;

    public virtual AnswerOption SelectedOption { get; set; } = null!;
}
