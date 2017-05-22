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

All locks provide both synchronous and asynchronous means of acquireing the locks.

Asynchronous locks do not take thread time when waiting for lock acquisition. This may improve general performance of your application by limiting the number of idle threads.

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

## AsyncLock
AsyncLock provides exclusive access to critical secion of code. Only one thread can acquire AsyncLock at a time:

    readonly IAsyncLock Lock = new AsyncLock();
    
    public async Task Example()
    {
        // acquire lock asynchronously
        using(await Lock.AcquireWriteLockAsync())
        {
            //  no other thread will have concurrent access to this critical section

            // it is safe to use async code inside critical section
            await Task.Delay(500);

            // but be carefult, you may now be on a different thread than thread which acquired the lock in a first place.
        }
    }

### Non Recursive AsyncLock
Default behavior AsyncLock is to be non recursive, this means that following code will cause **LockRecursionException** to be thrown on attempt to acquire alread held lock:

    readonly IAsyncLock Lock = new AsyncLock();
    
    public async Task Example()
    {
        // acquire lock asynchronously
        using(await Lock.AcquireWriteLockAsync())
        {
            // this will throw LockRecursionException
            using(await Lock.AcquireWriteLockAsync())
            {
                // ..
            }
        }
    }

It is easy to avoid writing code as above, but quite often second attemt to acquire a lock will be hidden somewhere in code tree:

    readonly IAsyncLock Lock = new AsyncLock();
    
    public async Task Example()
    {
        // acquire lock asynchronously
        using(await Lock.AcquireWriteLockAsync())
        {
            DoStomething();
        }
    }

    public void DoSomething()
    {
        using(Lock.AcquireWriteLock())
        {
            // ..
        }
    }

One way to avoid such scenarios is to have an internal, non-blocking version of methods:

    readonly IAsyncLock Lock = new AsyncLock();
    
    public async Task Example()
    {
        // acquire lock asynchronously
        using(await Lock.AcquireWriteLockAsync())
        {
            DoStomething_NOLOCK();
        }
    }

    public void DoSomething()
    {
        using(Lock.AcquireWriteLock())
        {
            DoSomething_NOLOCK();
        }
    }

    // this non-blocking version of .DoSomething() will only ever be called internally
    void DoSomething_NOLOCK()
    {
        // ..
    }

Another options is to allow lock recursion:

    readonly IAsyncLock Lock = new AsyncLock(recursionPolicy: LockRecursionPolicy.SupportsRecursion);
    
    public async Task Example()
    {
        // acquire lock asynchronously
        using(await Lock.AcquireWriteLockAsync())
        {
            // it is now safe to call DoSomething() and try to re-acquire same block
            DoStomething();
        }
    }

    public void DoSomething()
    {
        using(Lock.AcquireWriteLock())
        {
            // ..
        }
    }

### Read Locks
AsyncLock exposes IReadLock interface to allow read locks to be acquired.
This interface is for convinience only and allow AsyncLock to be used in scenarios where read/write lock would be used instead. Internally AsyncLock allows only singe read or single write happening at a same time. There is no priority given to either reads or writes.