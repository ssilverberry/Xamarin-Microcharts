using System;
using System.Collections.Generic;
using System.Text;

namespace App1
{
    class DeviceInfo
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int Sequence { get; set; }
        public int Rssi { get; set; }
        public int StationId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
