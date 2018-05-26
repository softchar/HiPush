using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hi.Repository.DataContextStorage
{
    public class ThreadDataContextStorageContainer : IDataContextStorageContainer
    {
        private static readonly Hashtable _libraryDataContexts = new Hashtable();

        public HiDataContext GetDataContext()
        {
            HiDataContext libraryDataContext = null;

            if (_libraryDataContexts.Contains(GetThreadName()))
                libraryDataContext = (HiDataContext)_libraryDataContexts[GetThreadName()];

            return libraryDataContext;
        }

        public void Store(HiDataContext context)
        {
            if (_libraryDataContexts.Contains(GetThreadName()))
                _libraryDataContexts[GetThreadName()] = context;
            else
                _libraryDataContexts.Add(GetThreadName(), context);
        }

        private static string GetThreadName()
        {
            return Thread.CurrentThread.ManagedThreadId.ToString();
        }
    }
}
