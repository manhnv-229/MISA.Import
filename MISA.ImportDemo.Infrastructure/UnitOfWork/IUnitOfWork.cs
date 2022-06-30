using MISA.ImportDemo.Infrastructure.DatabaseContext;
using MySqlConnector;
//using MySql.Data.MySqlClient;
using System;

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
