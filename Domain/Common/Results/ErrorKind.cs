﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.Results
{

    public enum ErrorKind
    {
        Failure,
        Unexpected,
        Validation,
        Conflict,
        NotFound,
        Unauthorized,
        Forbidden
    }

}
