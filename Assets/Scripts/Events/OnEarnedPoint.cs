using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
    public class EarnedPointEventArgs : EventArgs
    {
        private readonly int point;
        private readonly string reason;

        public int Point { get { return point; } }
        public string Reason { get { return reason; } }

        public EarnedPointEventArgs(int point, string reason)
        {
            this.point = point;
            this.reason = reason;
        }
    }
}
