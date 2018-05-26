using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Buffer.Base
{
    public class AllocException : Exception
    {
        public AllocException(string message)
            : base(message)
        {

        }
    }
}
