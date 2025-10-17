﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ZoomMeeting.Enum
{
    public enum RecordingStatus
    {
        Pending,
        Processing,
        Completed,
        Failed
    }

    public enum MeetingStatus
    {
        Waiting,
        Started,
        Completed
    }
}
