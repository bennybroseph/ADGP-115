using System.Collections.Generic;   // Used for 'List<T>'
using Library.Contextual;           // Used for 'Debug'

using Event = Define.Event;


namespace Library
{
    /// <summary>
    /// Singleton class which will allow you to subscribe to events and then broadcast those eventts which call delegates automatically.
    /// Assists in creating tightly coupled functions through an outside event system.
    /// <para>In order to use the 'DelayedBroadcast' function, you must call 'Update' in an update loop</para>
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

        private struct DelayedBroadcastData
        {
            public readonly Subscription m_Subscription;
            public readonly Event m_Event;
            public readonly object[] m_Params;

            public DelayedBroadcastData(Subscription a_Subscription, Event a_Event, object[] a_Params)
            {
                m_Subscription = a_Subscription;
                m_Event = a_Event;
                m_Params = a_Params;
            }
        }
        /// <summary>
        /// A collection which holds the data for firing an event after a delay
        /// </summary>
        private List<DelayedBroadcastData>  m_DelayedBroadcasts;

        public Publisher()
        {
            m_Subscriptions = new Dictionary<Event, Subscription>();

            m_DelayedBroadcasts = new List<DelayedBroadcastData>();
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
                Debug.Error("The message '" + a_Event + "' does not exist. You cannot unsubscribe from it.");
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
            {
                if (a_Params.Length == 0)
                    a_Params = null;
                callback(a_Event, a_Params);
            }
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
            {
                if (a_Params.Length == 0)
                    a_Params = null;
                m_DelayedBroadcasts.Add(new DelayedBroadcastData(callback, a_Event, a_Params));
            }
        }

        /// <summary>
        /// Update function which needs to be tied to the program it is running for
        /// <para>You must tie this to an update loop in order to call 'DelayedBroadcast'</para>
        /// </summary>
        public void Update()
        {
            foreach (var delayedBroadcastData in m_DelayedBroadcasts)
                delayedBroadcastData.m_Subscription(delayedBroadcastData.m_Event, delayedBroadcastData.m_Params);

            if(m_DelayedBroadcasts.Count != 0)
                m_DelayedBroadcasts = new List<DelayedBroadcastData>();
        }
    }
}
