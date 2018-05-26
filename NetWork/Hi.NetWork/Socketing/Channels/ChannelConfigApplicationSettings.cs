using Hi.Infrastructure.Configuration;
using Hi.NetWork.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Hi.NetWork.Channels {

    public class ChannelConfigApplicationSettings : WebConfigApplicationSettings {

        public override T GetSection<T>(string node) {

            T channelSetting = (T)ConfigurationManager.GetSection(node);

            return channelSetting;

        }

    }
}
