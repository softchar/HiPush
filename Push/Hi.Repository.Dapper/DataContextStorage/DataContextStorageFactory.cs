using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hi.Repository.Dapper.DataContextStorage
{
    public class DataContextStorageFactory<Object>
    {
        public static IStorageContainer<Object> _storageContainer;

        public static IStorageContainer<Object> CreateStorageContainer()
        {
            if (_storageContainer == null)
            {
                //这里实际应该根据配置文件来判定
                if (HttpContext.Current == null)
                    _storageContainer = new ThreadStorageContainer<Object>();
                else
                    _storageContainer = new HttpStorageContainer<Object>();
            }
            return _storageContainer;
        }
    }
}
