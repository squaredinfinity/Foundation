using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Foundation.Presentation.Xaml.UnitTests.Common.Collections
{
    [TestClass]
    public class ObservableCollectionExTests
    {
        [TestMethod]
        public void MyTestMethod()
        {
            return;
            Assert.Inconclusive();
            var w = (__Resources.ObservableCollection__BackgroundUpdatesTest)null;

            var r = new CrossThreadTestRunner();
            r.RunInSTA(() =>
                {
                    w = new __Resources.ObservableCollection__BackgroundUpdatesTest();

                    try
                    {
                        w.ShowDialog();

                        Parallel.For(0, 100, (i) =>
                            {
                                w.Items.Add(i);
                            });


                        Trace.WriteLine("lol");

                        //for (int i = 0; i < 100; i++)
                        //{
                        //    w.Items.Add(i);
                        //}
                    }
                    finally
                    {
                     //   w.Close();
                    }
                });

            Thread.Sleep(1000);

            Parallel.For(0, 100, (i) =>
            {
                w.Items.Add(i);
            });

            Thread.Sleep(10000);

            w.Dispatcher.Invoke(() => Assert.AreEqual(100, w._listView.Items.Count));
        }
    }

    public class CrossThreadTestRunner
    {
        private Exception lastException;

        public void RunInMTA(ThreadStart userDelegate)
        {
            Run(userDelegate, ApartmentState.MTA);
        }

        public void RunInSTA(ThreadStart userDelegate)
        {
            Run(userDelegate, ApartmentState.STA);
        }

        private void Run(ThreadStart userDelegate, ApartmentState apartmentState)
        {
            lastException = null;

            Thread thread = new Thread(
              delegate()
              {
                  try
                  {
                      userDelegate.Invoke();
                  }
                  catch (Exception e)
                  {
                      lastException = e;
                  }
              });
            thread.SetApartmentState(apartmentState);

            thread.Start();
            //thread.Join();

            if (ExceptionWasThrown())
                ThrowExceptionPreservingStack(lastException);
        }

        private bool ExceptionWasThrown()
        {
            return lastException != null;
        }

        [ReflectionPermission(SecurityAction.Demand)]
        private static void ThrowExceptionPreservingStack(Exception exception)
        {
            FieldInfo remoteStackTraceString = typeof(Exception).GetField(
              "_remoteStackTraceString",
              BindingFlags.Instance | BindingFlags.NonPublic);
            remoteStackTraceString.SetValue(exception, exception.StackTrace + Environment.NewLine);
            throw exception;
        }
    }

}
