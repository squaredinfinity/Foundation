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
        IBulkUpdate BeginBulkUpdate(CancellationToken ct);
        IBulkUpdate BeginBulkUpdate(TimeSpan timeout);
        IBulkUpdate BeginBulkUpdate(TimeSpan timeout, CancellationToken ct);
        IBulkUpdate BeginBulkUpdate(int millisecondsTimeout);
        IBulkUpdate BeginBulkUpdate(int millisecondsTimeout, CancellationToken ct);
        IBulkUpdate BeginBulkUpdate(SyncOptions options);


        Task<IBulkUpdate> BeginBulkUpdateAsync();
        Task<IBulkUpdate> BeginBulkUpdateAsync(CancellationToken ct);
        Task<IBulkUpdate> BeginBulkUpdateAsync(TimeSpan timeout);
        Task<IBulkUpdate> BeginBulkUpdateAsync(TimeSpan timeout, CancellationToken ct);
        Task<IBulkUpdate> BeginBulkUpdateAsync(int millisecondsTimeout);
        Task<IBulkUpdate> BeginBulkUpdateAsync(int millisecondsTimeout, CancellationToken ct); 
        Task<IBulkUpdate> BeginBulkUpdateAsync(AsyncOptions options);

        bool IsBulkUpdateInProgress();
    }
}
