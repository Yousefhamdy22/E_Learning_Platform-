using Infrastructure.ZoomServices.Dtos;
using Infrastructure.ZoomServices.RecordingService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Presentation.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class ZoomWebHookController : ControllerBase
    //{
    //    private readonly IRecordingService _recordingService;

    //    public ZoomWebHookController(IRecordingService recordingService)
    //    {
    //        _recordingService = recordingService;
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> Receive([FromBody] ZoomWebhookPayload payload, CancellationToken ct)
    //    {
    //        if (payload.Event != "recording.completed")
    //            return Ok();

    //        var obj = payload.Payload.Object;
    //        foreach (var file in obj.RecordingFiles)
    //        {
    //            await _recordingService.HandleRecordingCompletedAsync(
    //                meetingId: long.Parse(obj.Id),
    //                recordingId: file.Id,
    //                fileUrl: file.DownloadUrl,
    //                fileType: file.FileType,
    //                fileSize: file.FileSize,
    //                start: file.RecordingStart,
    //                end: file.RecordingEnd,
    //                ct
    //            );
    //        }

    //        return Ok();
    //    }
    //}

    [Route("api/[controller]")]
    [ApiController]
    public class ZoomWebHookController : ControllerBase
    {

        private readonly IRecordingService _recordingService;

        public ZoomWebHookController(IRecordingService recordingService)
        {
            _recordingService = recordingService;
        }
        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] ZoomWebhookPayload payload, CancellationToken ct)
        {
            Console.WriteLine($"📩 Received Zoom webhook: {payload.Event}");

            if (payload.Event != "recording.completed")
                return Ok();

            var obj = payload.Payload.Object;
            foreach (var file in obj.RecordingFiles)
            {
                await _recordingService.HandleRecordingCompletedAsync(
                    meetingId: long.Parse(obj.Id),
                    recordingId: file.Id,
                    fileUrl: file.DownloadUrl,
                    fileType: file.FileType,
                    fileSize: file.FileSize,
                    start: file.RecordingStart,
                    end: file.RecordingEnd,
                    ct
                );
            }

            return Ok();
        }
    }
}
