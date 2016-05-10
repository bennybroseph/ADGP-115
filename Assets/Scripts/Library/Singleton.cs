//////////////////////
//     Singleton    //
//////////////////////

namespace Library
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T s_Self;

        public static T self
        {
            get
            {
                if (s_Self == null)
                    s_Self = new T();
                return s_Self;
            }
        }

        protected Singleton() { }
    }
}

