using System;

namespace AGenius.UsefulStuff
{
    public static class EventHandlerExtensions
    {
        public static void SafeInvoke<T>(this EventHandler<T> evt, object sender, T e) where T : EventArgs
        {
            evt?.Invoke(sender, e);
        }
    }
}