﻿using System;
using System.Collections.Generic;

namespace Mix.Cms.Lib.Models.Cms
{
    public partial class MixMessengerUserDevice
    {
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        public string DeviceId { get; set; }
        public int Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
