using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Repository.Dapper.DataContextStorage
{
    public class ThreadStorageContainer<T> : IStorageContainer<T>
    {
        public T GetObject(string key)
        {
            throw new NotImplementedException();
        }

        public void Store(string key,T data)
        {
            throw new NotImplementedException();
        }
    }
}
