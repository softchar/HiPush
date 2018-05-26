using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Repository.DataContextStorage
{
    public class DataContextFactory
    {
        public static HiDataContext GetDataContext()
        {
            IDataContextStorageContainer _dataContextStorageContainer = DataContextStorageFactory.CreateStorageContainer();

            HiDataContext dataContext = _dataContextStorageContainer.GetDataContext();
            if (dataContext == null)
            {
                dataContext = new HiDataContext();
                _dataContextStorageContainer.Store(dataContext);
            }

            return dataContext;
        }
    }
}
