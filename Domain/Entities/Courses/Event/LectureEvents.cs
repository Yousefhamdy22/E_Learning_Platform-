using Domain.Common;
using Domain.Entities.Students;
using MediatR;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Courses.Event
{
    public sealed record LectureCreatedDomainEvent(Guid LectureId, Guid CourseId, DateTimeOffset ScheduledAt) : DomainEvent;

    public sealed record LectureEvents(Guid LectureId, string ContentUrl) : DomainEvent;

    public sealed record LectureRescheduledDomainEvent(Guid LectureId, DateTimeOffset NewScheduledAt) : DomainEvent;


 
    /// <summary>
    /// LectureCreatedDomainEventHandler
    ///Schedules Zoom meeting automatically.
    ///Sends notification to students/instructors.
    /// LectureCompletedDomainEventHandler
    ///  Gets recording from Zoom.
    /// Uploads to storage (e.g., Azure Blob).
    ///  Updates Lecture.ContentUrl.
    ///  LectureRescheduledDomainEventHandler
    ///
    ///   Updates calendar/integration services.
    ///   Notifies students/instructors.

}
