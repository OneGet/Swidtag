namespace Microsoft.PackageManagement.SwidTag.Test.Support {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class LocalEventSource : EventSource, IDisposable {
        protected internal List<Delegate> Delegates = new List<Delegate>();

        protected internal LocalEventSource() {
        }

        public LocalEventSource Events {
            get {
                return this;
            }
            set {
                return;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (Delegates != null) {
                foreach (var i in Delegates) {
                    XTask.CurrentExecutingTask.RemoveEventHandler(i);
                }

                Delegates = null;

                // encourage a bit of cleanup
                // todo: uh, what in the world is this?
                Task.Factory.StartNew(XTask.Collect);
            }
        }

        public static LocalEventSource operator +(LocalEventSource eventSource, Delegate eventHandlerDelegate) {
            XTask.CurrentExecutingTask.AddEventHandler(eventHandlerDelegate);
            eventSource.Delegates.Add(eventHandlerDelegate);
            return eventSource;
        }

        public static LocalEventSource Add(LocalEventSource eventSource, Delegate eventHandlerDelegate) {
            XTask.CurrentExecutingTask.AddEventHandler(eventHandlerDelegate);
            eventSource.Delegates.Add(eventHandlerDelegate);
            return eventSource;
        }

        public static LocalEventSource operator -(LocalEventSource eventSource, Delegate eventHandlerDelegate) {
            XTask.CurrentExecutingTask.RemoveEventHandler(eventHandlerDelegate);
            eventSource.Delegates.Remove(eventHandlerDelegate);
            return eventSource;
        }

        public static LocalEventSource Subtract(LocalEventSource eventSource, Delegate eventHandlerDelegate) {
            XTask.CurrentExecutingTask.RemoveEventHandler(eventHandlerDelegate);
            eventSource.Delegates.Remove(eventHandlerDelegate);
            return eventSource;
        }

        ~LocalEventSource() {
            Dispose(false);
        }
    }
}