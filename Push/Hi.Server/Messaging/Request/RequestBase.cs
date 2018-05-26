using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Server.Messaging.Request
{
    public class RequestBase
    {
        private int pageIndex = 0;
        public int PageIndex {
            get { return pageIndex; }
            set {
                if (value < 0)
                    value = 0;
                pageIndex = value;
            }
        }

        private byte pageSize = 20;
        public byte PageSize {
            get { return pageSize; }
            set {
                if (value <= 0)
                    value = pageSize;
                if (value > 200)
                    value = 200;
                pageSize = value; 
            }
        }
    }
}
