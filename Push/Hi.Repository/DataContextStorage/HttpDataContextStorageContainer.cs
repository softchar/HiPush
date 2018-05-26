using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hi.Repository.DataContextStorage
{
    public class HttpDataContextStorageContainer : IDataContextStorageContainer
    {
        private string _dataContextKey = "DataContext";

        /// <summary>
        /// 获得数据访问上下文
        /// </summary>
        /// <returns></returns>
        public HiDataContext GetDataContext()
        {
            HiDataContext context = null;
            if (HttpContext.Current.Items.Contains(_dataContextKey)) 
                context = (HiDataContext)HttpContext.Current.Items[_dataContextKey];
            return context;
        }

        /// <summary>
        /// 存储数据访问上下文
        /// </summary>
        /// <param name="context"></param>
        public void Store(HiDataContext context)
        {
            if (HttpContext.Current.Items.Contains(_dataContextKey))
                HttpContext.Current.Items[_dataContextKey] = context;
            else
                HttpContext.Current.Items.Add(_dataContextKey, context);
        }
    }
}
