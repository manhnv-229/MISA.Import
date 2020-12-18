using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Infrastructure.DatabaseContext
{
    public interface IDatabaseContext
    {
        MySqlConnection Connection { get; }
    }
}
