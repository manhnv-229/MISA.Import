using MISA.ImportDemo.Infrastructure.DatabaseContext;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDatabaseContextFactory _factory;
        private IDatabaseContext _context;
        public MySqlTransaction Transaction { get; private set; }

        //public UnitOfWork(IDatabaseContextFactory factory)
        //{
        //    _factory = factory;
        //}
        public UnitOfWork()
        {
            _factory = new DatabaseContextFactory();
        }

        /// <summary>
        /// Commit changes
        /// </summary>
        public void Commit()
        {
            if (Transaction != null)
            {
                try
                {
                    Transaction.Commit();
                }
                catch (Exception)
                {
                    Transaction.Rollback();
                }
                Transaction.Dispose();
                Transaction = null;
            }
            else
            {
                throw new NullReferenceException("Tryed commit not opened transaction (Transaction đã đóng hoặc chưa mở.)");
            }
        }

        /// <summary>
        /// Define a property of context class
        /// </summary>
        public IDatabaseContext DataContext
        {
            get { return _context ?? (_context = _factory.Context()); }
        }

        /// <summary>
        /// Begin a database transaction
        /// </summary>
        /// <returns>Transaction</returns>
        public MySqlTransaction BeginTransaction()
        {
            if (Transaction != null)
            {
                throw new NullReferenceException("Not finished previous transaction");
            }
            Transaction = _context.Connection.BeginTransaction();
            return Transaction;
        }

        /// <summary>
        /// RollBack changes
        /// </summary>
        public void RollBack()
        {
            if (Transaction != null)
            {
                try
                {
                    Transaction.Rollback();
                }
                catch (Exception)
                {
                    Transaction.Rollback();
                }
                Transaction.Dispose();
                Transaction = null;
            }
            else
            {
                throw new NullReferenceException("Tryed commit not opened transaction (Transaction đã đóng hoặc chưa mở.)");
            }
        }

        public void Dispose()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
            }
            if (_context != null)
            {
                _context.Connection.Close();
            }
        }
        
    }
}
