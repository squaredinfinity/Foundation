# Threading ▪ Locks

Locks library for .Net
Includes implementations of several async lock patterns.

## Getting Started
Install the [NuGet package](http://www.nuget.org/packages/SquaredInfinity.Threading.Locks).

    using SquaredInfinity.Threading.Locks;

    var _lock = new AsyncLock();

    // acquire lock asynchronously
    using(await _lock.AcquireWriteLockAsync())
    {
        //  no other thread will have concurrent access to this critical section

        // it is safe to use async code inside critical section
        await Task.Delay(500);
    }

    // lock is released outside of using statement

##  API Overview

All locks provide means of getting a disposable **lock acquisition**. The lock will be held until lock acquisition is disposed.

All locks expose both **Read** and **Write** modes on an api consumer level, but internal implementation may vary depending on the lock.

All locks provide both synchronous and asynchronous means of acquireing the locks. Wait for both sync and async lock can be given a timeout and a cancellation token.

Asynchronous locks do not use CPU time when waiting for lock acquisition. This may improve general performance of your application by limiting the number of idle threads and spin waits.

Available locks are:
- **AsyncLock** - asynchronous mutex which allows a single concurrent read or a single write.
- **AsyncReaderWriterLock** - asynchronous reader/writer lock which allows multiple concurrent reads or a single write.

### Recursion
Recursive (reentrant) locks allow same thread to acquire same lock multiple times.

Non-Recursive locks will fail to acquire same lock on the same thread more than once.

All locks are non-recursive by default but do allow recursive mode if needed.

It is much easier to write a dead-locking code using recursive locks so extra care must be taken with that approach.

### Lock Acquisition
Disposable **ILockAcquisition** will hold the lock in place until it is disposed. ILockAcqusition exposes **IsLockHeld** property which indicates if lock has been acquired succesfully. In some cases lock may not be acquired (e.g. when CancellationToken associated with acquisition has been cancelled, or timeout has expired).

It is possible to acquire multiple locks asynchronously:

    var l1 = new AsyncLock();
    var l2 = new AsyncLock();
    var l3 = new AsyncLock();
    var l4 = new AsyncLock();

    var all_acquisitions =
        await
        AsyncLock.AcquireWriteLockAsync(l1, l2, l3, l4);

    Assert.IsTrue(all_acquisitions.IsLockHeld);

### Composite Locks
It is possible to specify child locks for a lock.
For example, you may want to lock a whole branch of a tree, where each node in a tree has its own lock.
In that locking one node will succeed only when all child locks have also been acquired:

            var master = new AsyncLock();
            var c1 = new AsyncLock();
            var c2 = new AsyncLock();
            var c3 = new AsyncLock();
            var c4 = new AsyncLock();

            top.AddChild(c1);
            top.AddChild(c2);
            top.AddChild(c3);
            top.AddChild(c4);

            var a = await top.AcquireWriteLockAsync();

            // all locks are held: master, c1, c2, c3 and c4
            Assert.IsTrue(a.IsLockHeld);

            // all locks are released: master, c1, c2, c3 and c4
            a.Dispose();

If AsyncLock fails to acquire lock on any of its children, whole lock acquisition will fail and any locks acquired in the process will be released.

### await
.xxxAsync() methods can be used together with await keyword
    
    var _lock = new AsyncLock();
    
    using(await _lock.AcquireWriteLockAsync())
    {
        // ...
    }

Note that usual async/await concepts apply here and *.AcquireWriteLockAsync()* is **NOT** guaranteed to finish on a separate thread. For example, it will run synchronously if no lock is currently held and no wait is needed.

