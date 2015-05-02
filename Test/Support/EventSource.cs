namespace Microsoft.PackageManagement.SwidTag.Test.Support {
    using System;

    public class EventSource {
        public static EventSource Instance = new EventSource();

        protected internal EventSource() {
        }

        /// <summary>
        ///     Adds an event handler delegate to the current tasktask
        /// </summary>
        /// <param name="eventSource"> </param>
        /// <param name="eventHandlerDelegate"> </param>
        /// <returns> </returns>
        public static EventSource operator +(EventSource eventSource, Delegate eventHandlerDelegate) {
            XTask.CurrentExecutingTask.AddEventHandler(eventHandlerDelegate);
            return eventSource;
        }

        public static EventSource Add(EventSource eventSource, Delegate eventHandlerDelegate) {
            XTask.CurrentExecutingTask.AddEventHandler(eventHandlerDelegate);
            return eventSource;
        }

        public static EventSource operator -(EventSource eventSource, Delegate eventHandlerDelegate) {
            XTask.CurrentExecutingTask.RemoveEventHandler(eventHandlerDelegate);
            return eventSource;
        }

        public static EventSource Subtract(EventSource eventSource, Delegate eventHandlerDelegate) {
            XTask.CurrentExecutingTask.RemoveEventHandler(eventHandlerDelegate);
            return eventSource;
        }
    }
}