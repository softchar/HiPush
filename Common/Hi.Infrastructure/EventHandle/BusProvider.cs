using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.EventHandle {
    public class BusProvider {
        private static EventBus _instance;
        public static EventBus Instance {
            get {
                if (_instance == null) {
                    _instance = new EventBus();
                }

                return _instance;
            }
        }

        public BusProvider() {
        }
    }
}
