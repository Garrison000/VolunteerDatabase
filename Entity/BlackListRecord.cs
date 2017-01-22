﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolunteerDatabase.Entity
{
    public class BlackListRecord
    {
        public int Id { get; set; }
        public DateTime AddTime { get; set; }
        public DateTime EndTime { get; set; }
        public BlackListRecordStatus Status { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual AppUser Adder { get; set; }
        public virtual Volunteer Volunteer { get; set; }
        public virtual Project Project { get; set; }

    }
    public enum BlackListRecordStatus
    {
        Disabled,
        Enabled
    }
}
