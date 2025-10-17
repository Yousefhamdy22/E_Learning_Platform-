using Infrastructure.ZoomServices.RecordingService.BackgroundTask;
using Domain.Common.Interface;
using Domain.Entities.ZoomMeeting;
using Domain.Entities.ZoomMeeting.Enum;
using Infrastructure.GoogleDeriveServices;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.ZoomServices.RecordingService
{
    public class RecordingService : IRecordingService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RecordingService> _logger;
        private readonly HybridCache _cache;

        public RecordingService(
            IBackgroundTaskQueue taskQueue,
            IGoogleDriveService googleDriveService,
            ILogger<RecordingService> logger,
        IUnitOfWork unitOfWork,
            HybridCache cache)
        {
            _taskQueue = taskQueue;
            _googleDriveService = googleDriveService;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task HandleRecordingCompletedAsync(long meetingId, string recordingId,
                                                        string fileUrl, string fileType,
                                                        long fileSize, DateTime start,
                                                         DateTime end, CancellationToken ct)

        {
            var zoomMeeting = await _unitOfWork.ZoomMeetings
                                               .FindAsync(m => m.ZoomMeetingId == meetingId);
            _logger.LogWarning("MeetingId May Be Null", zoomMeeting);
            if (zoomMeeting == null) return;

            var recording = ZoomRecording.Create(
                recordingId, meetingId, fileUrl, fileType, fileSize,
                (int)(end - start).TotalSeconds, start, end); // ZoomMeeting

            await _unitOfWork.ZoomRecordes.AddAsync(recording, ct);
            await _unitOfWork.CommitAsync(ct);

         
            await _cache.SetAsync($"recording:{recordingId}", new { status = "Pending" });

        
            await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                try
                {
                    _logger.LogInformation("Start Queue processing");
                    var driveLink = await _googleDriveService
                        .UploadRecordingAsync(fileUrl, $"{meetingId}-{recordingId}.{fileType}");

                    recording.UpdateStatus(RecordingStatus.Processing, processedUrl: driveLink);
                    await _unitOfWork.CommitAsync(token);

                    // Cache final state
                    await _cache.SetAsync($"recording:{recordingId}", new { status = "Processed", url = driveLink });
                }
                catch (Exception ex)
                {
                    _logger.LogCritical("exist Problem For Queueing process ", ex.Message);
                    recording.UpdateStatus(RecordingStatus.Failed);
                    await _unitOfWork.CommitAsync(token);

                    await _cache.SetAsync($"recording:{recordingId}", new { status = "Failed", error = ex.Message });
                }
            });
        }
    }

}
