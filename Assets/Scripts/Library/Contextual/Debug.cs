#if (!UNITY_EDITOR && DEBUG)
using System; // Require for 'Console'
#endif

using UnityDebug = UnityEngine.Debug;

namespace Library.Contextual
{
    internal static class Debug
    {
        public static void Message(object a_Message)
        {
#if !UNITY_EDITOR && DEBUG
                Console.WriteLine(a_Message);
#elif UNITY_EDITOR && DEBUG
            UnityDebug.Log(a_Message);
#endif
        }
        public static void Warning(object a_Message)
        {
#if !UNITY_EDITOR && DEBUG
                Console.WriteLine(a_Message + "...");
#elif UNITY_EDITOR && DEBUG
            UnityDebug.LogWarning(a_Message + "...");
#endif
        }
        public static void Error(object a_Message)
        {
#if !UNITY_EDITOR && DEBUG
                Console.WriteLine("ERROR: " + a_Message + "!");
#elif UNITY_EDITOR && DEBUG
            UnityDebug.LogError(a_Message + "!");
#endif
        }
    }

}
