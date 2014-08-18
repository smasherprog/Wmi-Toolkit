using System;
using System.Collections.Generic;
using System.Text;

namespace Wmi.Process
{
    public struct ProcessPriority
    {
        public enum priority:uint 
        {
            IDLE = 0x40,
            BELOW_NORMAL = 0x4000,
            NORMAL = 0x20,
            ABOVE_NORMAL = 0x8000,
            HIGH_PRIORITY = 0x80,
            REALTIME = 0x100
        }
    }
}
