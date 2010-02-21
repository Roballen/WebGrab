using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ArtOfTest.WebAii.ObjectModel;

namespace ConsoleGrabit
{
    public class MethodTimeOut
    {
        public static void CallAndWait(Action action, int timeout)
        {

            Thread subThread = null;
            Action wrappedAction = () =>
            {
                subThread = Thread.CurrentThread;
                action();
            };

            var result = wrappedAction.BeginInvoke(null, null);

            if (((timeout != -1) && !result.IsCompleted) && (!result.AsyncWaitHandle.WaitOne(timeout, false) || !result.IsCompleted))
            {
                if (subThread != null)
                {
                    subThread.Abort();
                }

                throw new TimeoutException();
            }
            else
            {
                action.EndInvoke(result);
            }
        }
        public static void CallAndWait(Action<Element> action, Element el, int timeout)
        {
            Thread subThread = null;
            Action wrappedAction = () =>
            {
                subThread = Thread.CurrentThread;
                action(el);
            };

            var result = wrappedAction.BeginInvoke(null, null);

            if (((timeout != -1) && !result.IsCompleted) && (!result.AsyncWaitHandle.WaitOne(timeout, false) || !result.IsCompleted))
            {
                if (subThread != null)
                {
                    subThread.Abort();
                }

                throw new TimeoutException();
            }
            else
            {
                action.EndInvoke(result);
            }
        }
    }
}
