﻿using System;
using System.Collections.Generic;

namespace Mix.Cms.Messenger.Models.Data
{
    public partial class MixMessengerTeam
    {
        public MixMessengerTeam()
        {
            MixMessengerMessage = new HashSet<MixMessengerMessage>();
            MixMessengerNavTeamUser = new HashSet<MixMessengerNavTeamUser>();
        }

        public int Id { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public string HostId { get; set; }
        public bool? IsOpen { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }

        public ICollection<MixMessengerMessage> MixMessengerMessage { get; set; }
        public ICollection<MixMessengerNavTeamUser> MixMessengerNavTeamUser { get; set; }
    }
}
