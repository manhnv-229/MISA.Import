using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces.Base
{
    public interface IEntityRepository
    {
        Task<IReadOnlyList<T>> GetAllEntities<T>();
        Task<T> GetEntityByIdAsync<T>(object id);
        Task<IReadOnlyList<T>> GetEntities<T>(string procedureName, object[] parameters);
    }
}
