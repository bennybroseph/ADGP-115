//////////////////////
//     Singleton    //
//////////////////////

using UnityEngine;

namespace BennyBroseph.Contextual
{
#if UNITY_EDITOR
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool s_IsQuitting;
#else
        public abstract class Singleton<T> where T : class, new()
        {
#endif

        private static T s_Self;

        public static T self
        {
            get
            {
                if (s_IsQuitting)
                    return null;

                if (s_Self == null)
#if UNITY_EDITOR
                    s_Self = FindObjectOfType<T>();
#else
                        s_Self = new T();
#endif
                return s_Self;
            }
        }

        protected Singleton()
        {

        }

#if UNITY_EDITOR
        public void OnDestroy()
        {
            s_IsQuitting = true;
        }
#endif
    }
}

