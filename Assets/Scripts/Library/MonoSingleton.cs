//////////////////////
//   MonoSingleton  //
//////////////////////

using UnityEngine;

namespace Library
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool s_IsQuitting;

        private static T s_Self;

        public static T self
        {
            get
            {
                if (s_IsQuitting)
                    return null;

                if (s_Self == null)
                    s_Self = FindObjectOfType<T>();
                return s_Self;
            }
        }

        protected MonoSingleton() { }

        protected virtual void Awake()
        {
            s_IsQuitting = false;
        }

        protected virtual void OnDestroy()
        {
            s_IsQuitting = true;
            s_Self = null;
        }
    }
}