using Domain.Common;
using Domain.Entities.ZoomMeeting.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ZoomMeeting
{
    public class ZoomRecording : AuditableEntity
    {
        #region Prop

        
        public string RecordingId { get; private set; } = default!;
        public long MeetingId { get; private set; } = default!;
        public string FileUrl { get; private set; } = default!;
        public string FileType { get; private set; } = default!;
        public long FileSize { get; private set; }
        public int Duration { get; private set; }
        public DateTime RecordingStart { get; private set; }
        public DateTime RecordingEnd { get; private set; }
        public RecordingStatus Status { get; private set; } = RecordingStatus.Pending;
        public string? ProcessedUrl { get; private set; }
        public string? ThumbnailUrl { get; private set; }

        // Additional
        public string? GoogleDriveFileId { get; private set; } // For Google Drive reference
        public string? GoogleDriveUrl { get; private set; } // Shareable link for students
        public string? ProcessingError { get; private set; } // Track failures
        public int RetryCount { get; private set; } // For retry logic

        // Foreign key
        public long ZoomMeetingId { get; private set; }

        // Navigation property
        public ZoomMeeting ZoomMeeting { get; private set; } = default!;

        #endregion

        #region Factory Method 

        //, long zoomMeetingId
        public static ZoomRecording Create(string recordingId, long meetingId, string fileUrl,
                                         string fileType, long fileSize, int duration,
                                         DateTime recordingStart, DateTime recordingEnd)
        {
            return new ZoomRecording
            {
                RecordingId = recordingId,
                MeetingId = meetingId,
                FileUrl = fileUrl,
                FileType = fileType,
                FileSize = fileSize,
                Duration = duration,
                RecordingStart = recordingStart,
                RecordingEnd = recordingEnd,
                //ZoomMeetingId = zoomMeetingId
            };
        }
        #endregion

        #region Behavoirs

   
        public void UpdateStatus(RecordingStatus status, string? processedUrl = null, string? thumbnailUrl = null)
        {
            Status = status;
            if (processedUrl != null)
                ProcessedUrl = processedUrl;
            if (thumbnailUrl != null)
                ThumbnailUrl = thumbnailUrl;
        }

        public void MarkAsProcessing()
        {
            Status = RecordingStatus.Processing;
        }

        public void MarkAsCompleted(string googleDriveFileId, string googleDriveUrl)
        {
            Status = RecordingStatus.Completed;
            GoogleDriveFileId = googleDriveFileId;
            GoogleDriveUrl = googleDriveUrl;
            ProcessedUrl = googleDriveUrl; // Or keep separate?
        }

        public void MarkAsFailed(string error)
        {
            Status = RecordingStatus.Failed;
            ProcessingError = error;
        }

        #endregion
    }
}
