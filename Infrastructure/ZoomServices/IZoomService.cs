
using Domain.Common.Results;
using Domain.Entities.ZoomMeeting;
using Infrastructure.ZoomServices.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.ZoomServices
{
    public interface IZoomService
    {
        Task<ZoomMeetingResponse> CreateMeetingAsync(ZoomMeetingRequest request, CancellationToken ct);
        Task<ZoomMeeting> GetMeetingAsync(long zoomMeetingId, CancellationToken ct);


        //Task<Result<ZoomMeetingResponse>> GetMeetingAsync(string meetingId, CancellationToken ct);
        //Task<Result<List<ZoomMeetingResponse>>> GetUserMeetingsAsync(string userId, CancellationToken ct);
        //Task<Result<Updated>> UpdateMeetingAsync(string meetingId, ZoomMeetingRequest request, CancellationToken ct);
        //Task<Result<Deleted>> DeleteMeetingAsync(string meetingId, CancellationToken ct);
        //Task<Result<List<ZoomRecordingResponse>>> GetRecordingsAsync(string meetingId, CancellationToken ct);
        //Task<Result<Stream>> DownloadRecordingAsync(string downloadUrl, CancellationToken ct);

    }
}
