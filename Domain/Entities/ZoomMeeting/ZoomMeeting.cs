using Domain.Common;
using Domain.Common.Results;
using Domain.Entities.Courses;
using Domain.Entities.ZoomMeeting.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ZoomMeeting
{
    public class ZoomMeeting : AuditableEntity
    {
        public long ZoomMeetingId { get;  set; } = default!; 
        public string Topic { get;  set; } = default!;
        public DateTime StartTime { get;  set; }
        public int Duration { get;  set; } // minutes
        public string JoinUrl { get;  set; } = default!;
        public string StartUrl { get;  set; } = default!;
        public string Password { get;  set; } = default!;
        public string Status { get;  set; } = "waiting";

        // Foreign keys
        //public Guid HostId { get;  set; } // Teacher ID
        public Guid? CourseId { get;  set; }

        // Navigation properties
        //public virtual Instructor Host { get; private set; } = default!;


        public virtual Course? Course { get; private set; }
        public virtual ICollection<ZoomRecording> Recordings { get; private set; } = new List<ZoomRecording>();

        public ZoomMeeting() { }

        public ZoomMeeting(long zoomMeetingId, string topic, DateTime startTime,
            int duration, string joinUrl, string startUrl, string password,
            string status, Guid hostId, Guid? courseId, Instructor host, Course? course, 
            ICollection<ZoomRecording> recordings)
        {
            ZoomMeetingId = zoomMeetingId;
            Topic = topic;
            StartTime = startTime;
            Duration = duration;
            JoinUrl = joinUrl;
            StartUrl = startUrl;
            Password = password;
            Status = status;
         
            CourseId = courseId;
         
            Course = course;
            Recordings = recordings;
        }

        public static Result<ZoomMeeting> Create(long zoomMeetingId, string topic, DateTime startTime,
                                           int duration, string joinUrl, string startUrl,
                                           string password, Guid hostId, Guid? courseId = null)
        {
            //if (string.IsNullOrWhiteSpace(zoomMeetingId))
            //    return Result<ZoomMeeting>.FromError(Error.Validation("ZoomMeetingId required"));
            if (duration <= 0)
                return Result<ZoomMeeting>.FromError(Error.Validation("Duration must be > 0"));

            return Result<ZoomMeeting>.FromValue(new ZoomMeeting
            {
                ZoomMeetingId = zoomMeetingId,
                Topic = topic,
                StartTime = startTime,
                Duration = duration,
                JoinUrl = joinUrl,
                StartUrl = startUrl,
                Password = password,
             
                CourseId = courseId
            });
        }

        public void AddRecording(ZoomRecording recording)
        {
            if (recording == null) throw new ArgumentNullException(nameof(recording));
            Recordings.Add(recording);
        }

        public void StartMeeting() => Status = MeetingStatus.Started.ToString();
        public void CompleteMeeting() => Status = MeetingStatus.Completed.ToString();
        public void CancelMeeting() => Status = MeetingStatus.Waiting.ToString();
    }
}
