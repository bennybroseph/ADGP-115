using System; // Require for 'Console'
using UnityDebug = UnityEngine.Debug;

namespace Library.Contextual
{
    internal static class Debug
    {
        public static void Message(object a_Message)
        {
#if (!UNITY_EDITOR && DEBUG)
                Console.WriteLine(a_Message);
#elif UNITY_EDITOR
            UnityDebug.Log(a_Message);
#endif
        }
        public static void Warning(object a_Message)
        {
#if (!UNITY_EDITOR && DEBUG)
                Console.WriteLine(a_Message + "...");
#elif UNITY_EDITOR
            UnityDebug.LogWarning(a_Message + "...");
#endif
        }
        public static void Error(object a_Message)
        {
#if (!UNITY_EDITOR && DEBUG)
                Console.WriteLine("ERROR: " + a_Message + "!");
#elif UNITY_EDITOR
            UnityDebug.LogError(a_Message + "!");
#endif
        }
    }

}
