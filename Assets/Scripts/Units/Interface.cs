// Interface

using System;

namespace Units
{
    [Serializable]
    public struct Moving
    {
        public bool forward;
        public bool back;
        public bool left;
        public bool right;

        public static Moving nowhere = new Moving(false, false, false, false);

        public Moving(bool a_Forward, bool a_Back, bool a_Left, bool a_Right) : this()
        {
            forward = a_Forward;
            back = a_Back;
            left = a_Left;
            right = a_Right;
        }

        public static bool operator ==(Moving a_Left, Moving a_Right)
        {
            if (a_Left.forward == a_Right.forward &&
                a_Left.back == a_Right.back &&
                a_Left.left == a_Right.left &&
                a_Left.right == a_Right.right)
                return true;
            return false;
        }

        public static bool operator !=(Moving a_Left, Moving a_Right)
        {
            return !(a_Left == a_Right);
        }

        public override bool Equals(object a_Object)
        {
            return GetHashCode() == a_Object.GetHashCode();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
