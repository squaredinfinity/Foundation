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
        Task<IBulkUpdate> BeginBulkUpdateAsync(bool continueOnCapturedContext = false);
        Task<IBulkUpdate> BeginBulkUpdateAsync(CancellationToken ct, bool continueOnCapturedContext = false);
        Task<IBulkUpdate> BeginBulkUpdateAsync(int millisecondsTimeout, bool continueOnCapturedContext = false);
        Task<IBulkUpdate> BeginBulkUpdateAsync(int millisecondsTimeout, CancellationToken ct, bool continueOnCapturedContext = false);
        Task<IBulkUpdate> BeginBulkUpdateAsync(TimeSpan timeout, bool continueOnCapturedContext = false);
        Task<IBulkUpdate> BeginBulkUpdateAsync(TimeSpan timeout, CancellationToken ct, bool continueOnCapturedContext = false);

        bool IsBulkUpdateInProgress();
    }
}
