using MISA.ImportDemo.Infrastructure.DatabaseContext;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        IDatabaseContext DataContext { get; }
        MySqlTransaction BeginTransaction();
        void Commit();
        //void Dispose();

        void RollBack();
    }
}
