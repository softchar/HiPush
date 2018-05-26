using Hi.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Server.Model {

    public class ReceiveLog: IAggregateRoot { 

        public Guid Token { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public string Data { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public bool IsRemoved { get; set; }

        public byte Status { get; set; }

        public byte[] Version { get; set; }

        public ReceiveLog() {
            this.Token = Guid.NewGuid();
            this.CreateTime = DateTime.Now;
            this.LastUpdateTime = DateTime.Now;
            this.IsRemoved = false;
            this.Status = 0;
        }


    } 

}
