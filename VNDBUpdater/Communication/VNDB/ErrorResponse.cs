using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNDBUpdater.Communication.VNDB
{
    public enum ErrorResponse : byte
    {
        Throttled = 0,
        AuthenticationFailed,
        Unknown
    };
}
