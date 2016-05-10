using UnityEngine;

using System.Collections.Generic;

using BennyBroseph.Contextual;
using Collections;


namespace BennyBroseph
{
    public sealed class Publisher : Singleton<Publisher>
    {
        public delegate void Subscription(string a_Message, params object[] a_Params);

        private readonly Dictionary<string, Subscription> m_Messages;

        private List<Tuple<Subscription, string, object[]>>  m_DelayedCallbacks;

        public Publisher()
        {
            m_Messages = new Dictionary<string, Subscription>();

            m_DelayedCallbacks = null;
        }

        public void Subscribe(string a_Message, Subscription a_Subscription)
        {
            if (!m_Messages.ContainsKey(a_Message))
                m_Messages[a_Message] = a_Subscription;
            else
                m_Messages[a_Message] += a_Subscription;
        }
        public void UnSubscribe(string a_Message, Subscription a_Subscription)
        {
            if (m_Messages.ContainsKey(a_Message))
                m_Messages[a_Message] -= a_Subscription;
            else
                DebugError("The message '" + a_Message + "' does not exist. You cannot unsubscribe from it.");
        }

        public void Broadcast(string a_Message, params object[] a_Params)
        {
            Subscription callback;

            m_Messages.TryGetValue(a_Message, out callback);

            if (callback != null)
                callback(a_Message, a_Params);
        }

        public void DelayedBroadcast(string a_Message, params object[] a_Params)
        {
            Subscription callback;

            m_Messages.TryGetValue(a_Message, out callback);

            if (callback != null)
                m_DelayedCallbacks.Add(new Tuple<Subscription, string, object[]>(callback, a_Message, a_Params));
        }

        public void Update()
        {
            foreach (var tuple in m_DelayedCallbacks)
                tuple.Item1(tuple.Item2, tuple.Item3);

            if(m_DelayedCallbacks.Count != 0)
                m_DelayedCallbacks = new List<Tuple<Subscription, string, object[]>>();
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
