using UnityEngine;

using System.Collections.Generic;

using BennyBroseph.Contextual;
using Define;
using Collections;

using Event = Define.Event;


namespace BennyBroseph
{
    /// <summary>
    /// Singleton class which will allow you to subscribe to events and then broadcast those eventts which call delegates automatically.
    /// Assists in creating tightly coupled functions through an outside event system.
    /// </summary>
    public sealed class Publisher : Singleton<Publisher>
    {
        /// <summary>
        /// Base type of delegate which will be used to call functions after an event fires
        /// </summary>
        /// <param name="a_Event">The event which has fired/was subscribed to</param>
        /// <param name="a_Params">Optional parameters boxed as an object to send along with the event</param>
        public delegate void Subscription(Event a_Event, params object[] a_Params);

        /// <summary>
        /// A collection of all the delegates grouped by 'Event'
        /// </summary>
        private readonly Dictionary<Event, Subscription> m_Subscriptions;

        /// <summary>
        /// A collection which holds the data for firing an event after a delay
        /// </summary>
        private List<Tuple<Subscription, Event, object[]>>  m_DelayedCallbacks;

        public Publisher()
        {
            m_Subscriptions = new Dictionary<Event, Subscription>();

            m_DelayedCallbacks = new List<Tuple<Subscription, Event, object[]>>();
        }

        /// <summary>
        /// Subscribe to an event firing using a delegate
        /// </summary>
        /// <param name="a_Event">The event to subscribe to</param>
        /// <param name="a_Delegate">The delegate to call when the event fires</param>
        public void Subscribe(Event a_Event, Subscription a_Delegate)
        {
            if (!m_Subscriptions.ContainsKey(a_Event))
                m_Subscriptions[a_Event] = a_Delegate;
            else
                m_Subscriptions[a_Event] += a_Delegate;
        }
        /// <summary>
        /// UnSubscribe to an event firing.
        /// <para>Expects the delegate used to subscribe to be used when unsubscribing, otherwise bad things</para>
        /// </summary>
        /// <param name="a_Event">The event to unsubscribe to</param>
        /// <param name="a_Delegate">The delegate used when subscribing</param>
        public void UnSubscribe(Event a_Event, Subscription a_Delegate)
        {
            if (m_Subscriptions.ContainsKey(a_Event))
                m_Subscriptions[a_Event] -= a_Delegate;
            else
                DebugError("The message '" + a_Event + "' does not exist. You cannot unsubscribe from it.");
        }

        /// <summary>
        /// Fire off an event and call it's subsequent delegates immediatly
        /// </summary>
        /// <param name="a_Event">The event to broadcast</param>
        /// <param name="a_Params">Optional parameters boxed as an object to send along with the event</param>
        public void Broadcast(Event a_Event, params object[] a_Params)
        {
            Subscription callback;

            m_Subscriptions.TryGetValue(a_Event, out callback);

            if (callback != null)
                callback(a_Event, a_Params);
        }

        /// <summary>
        /// Fires off an event that will call it's subsequent delegates in the next 'Update' call rather than immediatly
        /// </summary>
        /// <param name="a_Event">The event to broadcast</param>
        /// <param name="a_Params">Optional parameters boxed as an object to send along with the event</param>
        public void DelayedBroadcast(Event a_Event, params object[] a_Params)
        {
            Subscription callback;

            m_Subscriptions.TryGetValue(a_Event, out callback);

            if (callback != null)
                m_DelayedCallbacks.Add(new Tuple<Subscription, Event, object[]>(callback, a_Event, a_Params));
        }

        /// <summary>
        /// Update function which needs to be tied to the program it is running for
        /// <para>You must tie this to an update loop in order to call 'DelayedBroadcast'</para>
        /// </summary>
        public void Update()
        {
            foreach (var tuple in m_DelayedCallbacks)
                tuple.Item1(tuple.Item2, tuple.Item3);

            if(m_DelayedCallbacks.Count != 0)
                m_DelayedCallbacks = new List<Tuple<Subscription, Event, object[]>>();
        }
        /// <summary>
        /// Attempts to access a debugging messenger. Will do nothing if it cannot be found
        /// </summary>
        /// <param name="a_Message">The message to display</param>
        private void DebugMessage(object a_Message)
        {
#if CONTEXT_DEBUG   // Only compiles if the build is using the 'ContextualDebug' by defining it in the build options
            Debug.Message(a_Message);
#elif (!UNITY_EDITOR && DEBUG) // Only compiles when in debug mode and not in unity
            Console.WriteLine(a_Message);
#endif
        }
        /// <summary>
        /// Attempts to access a debugging messenger at a warning level. Will do nothing if it cannot be found
        /// </summary>
        /// <param name="a_Message">The message to display</param>
        private void DebugWarning(object a_Message)
        {
#if CONTEXT_DEBUG   // Only compiles if the build is using the 'ContextualDebug' by defining it in the build options
            Debug.Warning(a_Message);
#elif (!UNITY_EDITOR && DEBUG) // Only compiles when in debug mode and not in unity
            Console.WriteLine(a_Message);
#endif
        }
        /// <summary>
        /// Attempts to access a debugging messenger at an error level. Will do nothing if it cannot be found
        /// </summary>
        /// <param name="a_Message">The message to display</param>
        private void DebugError(object a_Message)
        {
#if CONTEXT_DEBUG   // Only compiles if the build is using the 'ContextualDebug' by defining it in the build options
            Debug.Error(a_Message);
#elif (!UNITY_EDITOR && DEBUG) // Only compiles when in debug mode and not in unity
            Console.WriteLine(a_Message);
#endif
        }
    }
}
