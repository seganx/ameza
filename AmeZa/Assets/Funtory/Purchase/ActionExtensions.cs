﻿using System;
using UnityEngine;

namespace IabPlugin
{
    public static class ActionExtensions
    {
        private static void IabInvoke(Delegate listener, object[] args)
        {
            listener.Method.Invoke(listener.Target, args);
        }

        private static void IabInvokeListners(Delegate[] listeners, object[] args)
        {
            for (int i = 0; i < listeners.Length; ++i)
                IabInvoke(listeners[i], args);
        }

        public static void SafeInvoke(this Action action)
        {
            if (action == null)
                return;

            object[] args = { };
            Delegate[] listeners = action.GetInvocationList();

            IabInvokeListners(listeners, args);
        }

        public static void SafeInvoke<T1>(this Action<T1> action, T1 arg1)
        {
            if (action == null)
                return;

            object[] args = { arg1 };
            Delegate[] listeners = action.GetInvocationList();

            IabInvokeListners(listeners, args);

        }

        public static void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action == null)
                return;

            object[] args = { arg1, arg2 };
            Delegate[] listeners = action.GetInvocationList();

            IabInvokeListners(listeners, args);
        }

        public static void SafeInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (action == null)
                return;

            object[] args = { arg1, arg2, arg3 };
            Delegate[] listeners = action.GetInvocationList();

            IabInvokeListners(listeners, args);
        }
    }
}