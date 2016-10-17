using SquaredInfinity.Foundation.Maths.Statistics;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SquaredInfinity.Foundation.Collections;

namespace SquaredInfinity.Foundation.Threading
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class DiagnosticReaderWriterLockSlimEx : ReaderWriterLockSlimEx, IDiagnosticLock
    {
        readonly CompositeDisposable Disposables = new CompositeDisposable();

        readonly ISubject<double,double> SUBJECT__Read = Subject.Synchronize(new Subject<double>());
        readonly ISubject<double, double> SUBJECT__UpgradeableRead = Subject.Synchronize(new Subject<double>());
        readonly ISubject<double, double> SUBJECT__Write = Subject.Synchronize(new Subject<double>());

        public OnlineStatisticsInfo ReadStats { get; private set; } = new OnlineStatisticsInfo();
        public OnlineStatisticsInfo UpgradeableReadStats { get; private set; } = new OnlineStatisticsInfo();
        public OnlineStatisticsInfo WriteStats { get; private set; } = new OnlineStatisticsInfo();


        public DiagnosticReaderWriterLockSlimEx(string name)
            : base(name)
        {
            var s =
            OnlineStatistics.Calculate(
                SUBJECT__Read,
                KnownStatistics.All,
                new[]
                {
                    new AdditionalStatistic("sma5", x => new SimpleMovingAverage(5).Calculate(x).Select(_ => (object) _))
                }).Subscribe(x => ReadStats = x);
            Disposables.Add(s);

            s =
            OnlineStatistics.Calculate(
                SUBJECT__UpgradeableRead,
                KnownStatistics.All,
                new[]
                {
                    new AdditionalStatistic("sma5", x => new SimpleMovingAverage(5).Calculate(x).Select(_ => (object) _))
                 }).Subscribe(x => UpgradeableReadStats = x);
            Disposables.Add(s);

            s =
            OnlineStatistics.Calculate(
                SUBJECT__Write,
                KnownStatistics.All,
                new[]
                {
                    new AdditionalStatistic("sma5", x => new SimpleMovingAverage(5).Calculate(x).Select(_ => (object) _))
                }).Subscribe(x => WriteStats = x);
            Disposables.Add(s);
        }

        #region Read

        public override IReadLockAcquisition AcquireReadLock()
        {
            var sw = Stopwatch.StartNew();
            var l = base.AcquireReadLock();
            sw.Stop();
            SUBJECT__Read.OnNext(sw.Elapsed.TotalMilliseconds);
            return l;
        }

        public override IReadLockAcquisition AcquireReadLockIfNotHeld()
        {
            var sw = Stopwatch.StartNew();
            var l = base.AcquireReadLockIfNotHeld();
            sw.Stop();
            SUBJECT__Read.OnNext(sw.Elapsed.TotalMilliseconds);
            return l;
        }

        public override bool TryAcquireReadLock(TimeSpan timeout, out IReadLockAcquisition readLockAcquisition)
        {
            var sw = Stopwatch.StartNew();
            var r = base.TryAcquireReadLock(timeout, out readLockAcquisition);
            sw.Stop();
            SUBJECT__Read.OnNext(sw.Elapsed.TotalMilliseconds);
            return r;
        }

        #endregion

        #region Upgradeable Read

        public override IUpgradeableReadLockAcquisition AcquireUpgradeableReadLock()
        {
            var sw = Stopwatch.StartNew();
            var l = base.AcquireUpgradeableReadLock();
            sw.Stop();
            SUBJECT__UpgradeableRead.OnNext(sw.Elapsed.TotalMilliseconds);
            return l;
        }

        public override bool TryAcquireUpgradeableReadLock(TimeSpan timeout, out IUpgradeableReadLockAcquisition upgradeableReadLockAcquisition)
        {
            var sw = Stopwatch.StartNew();
            var r = base.TryAcquireUpgradeableReadLock(timeout, out upgradeableReadLockAcquisition);
            sw.Stop();
            SUBJECT__UpgradeableRead.OnNext(sw.Elapsed.TotalMilliseconds);
            return r;
        }

        #endregion

        #region Write

        public override IWriteLockAcquisition AcquireWriteLock()
        {
            var sw = Stopwatch.StartNew();
            var l = base.AcquireWriteLock();
            sw.Stop();
            SUBJECT__Write.OnNext(sw.Elapsed.TotalMilliseconds);
            return l;
        }

        public override IWriteLockAcquisition AcquireWriteLockIfNotHeld()
        {
            var sw = Stopwatch.StartNew();
            var l = base.AcquireWriteLockIfNotHeld();
            sw.Stop();
            SUBJECT__Write.OnNext(sw.Elapsed.TotalMilliseconds);
            return l;
        }

        public override bool TryAcquireWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeableLockAcquisition)
        {
            var sw = Stopwatch.StartNew();
            var r = base.TryAcquireWriteLock(timeout, out writeableLockAcquisition);
            sw.Stop();
            SUBJECT__Write.OnNext(sw.Elapsed.TotalMilliseconds);
            return r;
        }

        #endregion

        public string DebuggerDisplay
        {
            get
            {
                return $"{Name} {UniqueId}, r: {IsReadLockHeld}, ur: {IsUpgradeableReadLockHeld}, w: {IsWriteLockHeld}";
            }
        }
    }
}
