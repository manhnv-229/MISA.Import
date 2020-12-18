using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Infrastructure.DatabaseContext
{
    public class DatabaseContextFactory: IDatabaseContextFactory
    {
        private IDatabaseContext dataContext;

        /// <summary>
        /// Khởi tạo mới 1 Db Context nếu chưa có
        /// </summary>
        /// <returns>dataContext</returns>
        /// CreateBy: NVMANH (14/04/2020)
        public IDatabaseContext Context()
        {
            return dataContext ?? (dataContext = new DatabaseContext());
        }

        /// <summary>
        /// Dispose existing context
        /// </summary>
        public void Dispose()
        {
            //if (dataContext != null)
            //    dataContext.Disposable();
        }
    }
}
