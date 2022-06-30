//using MySql.Data.MySqlClient;
using MySqlConnector;
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
