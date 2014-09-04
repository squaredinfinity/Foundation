using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    //[TestClass]
    //public class Object__CreateWeakEventHandler
    //{
    //    [TestMethod]
    //    public void ShortLivedAttachesToLongLived__ShortLivedDies__EventIsUnhooked()
    //    {
    //        var o_short = new ShortLivedEntity();
    //        var o_long = new LongLivedEntity();

    //        var are = new AutoResetEvent(initialState: false);

    //        o_short.AttachToLong(o_long, are);


    //        Task.Factory.StartNew(() =>
    //            {
    //                Task.Delay(100);
    //                o_long.MakeSomethingHappen();
    //            });

    //        are.WaitOne();

    //        o_short = null;

    //        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
    //        GC.WaitForPendingFinalizers();
            
    //        Task.Factory.StartNew(() =>
    //        {
    //            Task.Delay(100);
    //            o_long.MakeSomethingHappen();
    //        });

    //        are.WaitOne();

    //        var x = "";

            
    //    }

    //    //public class V1_WeakEventHandler<TArgs>
    //    //    where TArgs : EventArgs
    //    //{
    //    //    private WeakReference<object> Target_ref;
    //    //    private MethodInfo MethodInfo;
    //    //    private EventHandler<TArgs> Handler;

    //    //    public V1_WeakEventHandler(EventHandler<TArgs> eventHandler)
    //    //    {
    //    //        Target_ref = new WeakReference<object>(eventHandler.Target);

    //    //        MethodInfo = eventHandler.Method;

    //    //        Handler = Invoke;
    //    //    }

    //    //    public void Invoke(object sender, TArgs e)
    //    //    {
    //    //        var target = (object)null;
                
    //    //        if(Target_ref.TryGetTarget(out target))
    //    //            MethodInfo.Invoke(target, new object[] { sender, e });
    //    //    }

    //    //    public static implicit operator EventHandler<TArgs>(V1_WeakEventHandler<TArgs> weakEventHandler)
    //    //    {
    //    //        return weakEventHandler.Handler;
    //    //    }
    //    //}

        

       

    //    public class ShortLivedEntity
    //    {
    //        public event EventHandler AfterShortLivedEntitySomethingHappened;

    //        public void MakeSomethingHappen()
    //        {
    //            if (AfterShortLivedEntitySomethingHappened != null)
    //                AfterShortLivedEntitySomethingHappened(this, EventArgs.Empty);
    //        }

    //        IDisposable LongSubscription;

    //        public void AttachToLong(LongLivedEntity l, AutoResetEvent are)
    //        {
    //            LongSubscription =
    //                l.CreateWeakEventHandler()
    //                .ForEvent<EventHandler, EventArgs>(
    //                (s, h) => s.AfterLongLivedEntitySomethingHappened += h,
    //                (s, h) => s.AfterLongLivedEntitySomethingHappened -= h)
    //                .Subscribe((_s, _e) => are.Set());
    //        }

    //        ~ShortLivedEntity()
    //        {

    //        }
    //    }

    //    public class MyDisposable : IDisposable
    //    {

    //        public void Dispose()
    //        {
                
    //        }

    //        ~MyDisposable()
    //        {

    //        }
    //    }

    //    public class LongLivedEntity
    //    {
    //        public event EventHandler AfterLongLivedEntitySomethingHappened;

    //        public void MakeSomethingHappen()
    //        {
    //            if (AfterLongLivedEntitySomethingHappened != null)
    //                AfterLongLivedEntitySomethingHappened(this, EventArgs.Empty);
    //        }
    //    }
    //}
}
