using SquaredInfinity.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public interface ISupportsAsyncBulkUpdate
    {
        IBulkUpdate BeginBulkUpdate();
        IBulkUpdate BeginBulkUpdate(SyncOptions options);


        Task<IBulkUpdate> BeginBulkUpdateAsync();
        Task<IBulkUpdate> BeginBulkUpdateAsync(AsyncOptions options);

        bool IsBulkUpdateInProgress();
    }
}
