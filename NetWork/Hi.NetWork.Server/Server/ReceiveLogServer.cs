using Hi.NetWork.Server.Model;
using Hi.NetWork.Server.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.NetWork.Server.Server {
    public class ReceiveLogServer {

        private ReceiveLogRepository repository;

        public ReceiveLogServer() {

            repository = new ReceiveLogRepository();

        }

        public void Log(ReceiveLog log) {

            repository.Add(log);

        }

    }
}
