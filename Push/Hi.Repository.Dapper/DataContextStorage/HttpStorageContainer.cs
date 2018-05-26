using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hi.Repository.Dapper.DataContextStorage
{
    public class HttpStorageContainer<T> : IStorageContainer<T>
    {

        public T GetObject(string key)
        {
            T context = default(T);
            if (HttpContext.Current.Items.Contains(key))
                context = (T)HttpContext.Current.Items[key];
            return context;
        }

        public void Store(string key, T data)
        {
            if (HttpContext.Current.Items.Contains(key))
                HttpContext.Current.Items[key] = data;
            else
                HttpContext.Current.Items.Add(key, data);
        }
    }
}
