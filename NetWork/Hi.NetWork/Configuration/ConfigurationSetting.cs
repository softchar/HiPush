using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Hi.Infrastructure.Base;
using System.Xml.Linq;

namespace Hi.NetWork.Configuration {

    public class ConfigurationSetting: IConfigurationSectionHandler {
        
        /// <summary>
        /// 缓冲区的种类
        /// </summary>
        public int BufferTypeCounter = 2;

        /// <summary>
        /// 一个socket的接收缓冲区的大小
        /// </summary>
        public int SocketReceiveBufferSize = 1024 * 64;

        /// <summary>
        /// 一个socket的发送缓冲区的大小
        /// </summary>
        public int SocketSendBufferSize = 1024 * 16;

        /// <summary>
        /// 最大并发数
        /// </summary>
        public int MaxConcurrentNumber = 65535;

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnectionNumber = 100000;

        /// <summary>
        /// 监听Socket的处理队列长度
        /// </summary>
        public int SocketLinsenQueueLength = 50000;

        /// <summary>
        /// 监听的地址
        /// </summary>
        public string Host = "127.0.0.1";

        /// <summary>
        /// 监听的端口
        /// </summary>
        public int Port = 23456;

        public object Create(object parent, object configContext, XmlNode section)
        {

            Ensure.IsNotNull(section);
            Ensure.Ensures(section.HasChildNodes);

            return transToConfigurationSetting(section);

        }

        private ConfigurationSetting transToConfigurationSetting(XmlNode section)
        {

            StringWriter _sw = new StringWriter();

            var _cfg = section.ChildNodes.OfType<XmlNode>().Where(node => node.Name.Equals(this.GetType().Name)).FirstOrDefault();

            _cfg.WriteTo(new XmlTextWriter(_sw));

            Stream _stream = new MemoryStream(Encoding.UTF8.GetBytes(_sw.ToString()));

            var _xmlSeriallizer = new XmlSerializer(typeof(ConfigurationSetting));

            var _setting = (ConfigurationSetting)_xmlSeriallizer.Deserialize(_stream);

            _stream.Close();
            _stream.Dispose();

            return _setting;
            
        }
    }
}
