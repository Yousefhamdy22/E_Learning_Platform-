using Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours.Interfaces
{
    public interface IZoomWebhookService
    {
      //  Task<Result> HandleRecordingCompletedEvent(ZoomWebhookEvent webhookEvent, CancellationToken ct = default);
        Task<bool> ValidateWebhookSignature(string payload, string signature, string secret);
    }
}
