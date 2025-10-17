using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Exams.Events
{
    public sealed record ExamCreatedDomainEvent(Guid ExamId, Guid CourseId, DateTimeOffset ScheduledAt) : DomainEvent;
    public sealed record ExamCompletedDomainEvent(Guid studentId , DateTimeOffset ComplateAt) : DomainEvent;
    public sealed record ExamRescheduledDomainEvent(Guid ExamId, DateTimeOffset NewScheduledAt) : DomainEvent;
    public sealed record ExamAutoGradedDomainEvent(Guid ExamId, Guid StudentId, double Score) : DomainEvent;
}
