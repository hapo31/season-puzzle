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
        private readonly Block.KIND kind;
        private readonly int count;

        public Block.KIND Kind
        {
            get
            {
                return kind;
            }
        }

        public int Point
        {
            get
            {
                return point;
            }
        }

        public string Reason
        {
            get
            {
                return reason;
            }
        }

        public int Count => count;

        public EarnedPointEventArgs(int count, int point, string reason, Block.KIND kind)
        {
            this.point = point;
            this.reason = reason;
            this.kind = kind;
            this.count = count;
        }
    }
}
