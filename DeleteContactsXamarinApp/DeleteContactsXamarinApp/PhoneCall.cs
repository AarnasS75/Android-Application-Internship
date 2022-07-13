using System;
using System.Collections.Generic;
using System.Text;

namespace HGB
{
    public class PhoneCall
    {
        public string deviceno { get; set; }
        public string phoneno { get; set; }
        public string direction { get; set; }
        public string dt { get; set; }
        public string action { get; set; }

        public PhoneCall(string deviceno, string phoneno, string direction, string dt, string action)
        {
            this.deviceno = deviceno;
            this.direction = direction;
            this.phoneno = phoneno;
            this.dt = dt;
            this.action = action;
        }
    }
}
