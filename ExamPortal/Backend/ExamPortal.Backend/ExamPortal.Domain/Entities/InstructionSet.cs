using System;
using System.Collections.Generic;

namespace ExamPortal.Domain.Entities;

public partial class InstructionSet
{
    public Guid Id { get; set; }

    public Guid ExamSetId { get; set; }

    public string Content { get; set; } = null!;

    public bool AgreementRequired { get; set; }

    public virtual ExamSet ExamSet { get; set; } = null!;
}
