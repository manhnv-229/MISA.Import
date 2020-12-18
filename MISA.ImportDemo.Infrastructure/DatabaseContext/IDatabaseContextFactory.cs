using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Infrastructure.DatabaseContext
{
    public interface IDatabaseContextFactory
    {
        IDatabaseContext Context();
    }
}
