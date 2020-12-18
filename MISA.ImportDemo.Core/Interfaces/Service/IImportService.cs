using Microsoft.AspNetCore.Http;
using MISA.ImportDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces
{
    public interface IImportService : IBaseEntityService<ImportFileTemplate>
    {
        Task<ActionServiceResult> Import(string importKey, bool overriderData, CancellationToken cancellationToken);
    }
}
