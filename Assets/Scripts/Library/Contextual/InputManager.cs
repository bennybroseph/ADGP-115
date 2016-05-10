using System.Collections.Generic;
using Library;
using UnityEngine;

namespace Library.Contextual
{
    /// <summary>
    /// <para> Hope you wanted to use Windows Forms or Unity
    /// Does not support standard C# because it would be less efficient to try to stream-line it. </para>
    /// WINDOWS FORM: You MUST call 'Init' and pass the main 'Form' object
    /// <para> UNITY: You MUST create an object which uses this script. Everything else will happen automatically</para>
    /// </summary>
    public sealed class InputManager : MonoSingleton<InputManager>
    {
        private static bool s_Initialized;

        public delegate void OnKeyDelegate(Keys a_Key);
        public delegate void OnMouseDelegate(int a_Button, Vector2 a_Position);

        private OnKeyDelegate m_OnKeyDown;
        private OnKeyDelegate m_OnKeyUp;

        private OnMouseDelegate m_OnMouseDown;
        private OnMouseDelegate m_OnMouseUp;
        private OnMouseDelegate m_OnMouseMove;

        private Vector2 m_PrevMousePosition;

        public List<Keys> heldKeys { get; private set; }
        public List<int> heldMouseButtons { get; private set; }

#if !UNITY_EDITOR && FORMS
        public InputManager()
        {
            s_Initialized = false;

            m_HeldKeys = new List<Keys>();
        }

        public void Init(System.Windows.Forms.Form a_Form)
        {
            if (!s_Initialized)
            {
                m_OnKeyUp = null;
                m_OnKeyDown = null;

                m_OnMouseDown = null;
                m_OnMouseUp = null;

                a_Form.KeyDown += KeyDownHandler;
                a_Form.KeyUp += KeyUpHandler;
                
                m_PrevMousePosition = new Vector2();
                
                heldKeys = new List<Keys>();
                heldMouseButtons = new List<int>();        

                s_Initialized = true;
            }
        }

        public void KeyDownHandler(object a_Sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!m_HeldKeys.Contains((Keys)e.KeyCode))
            {
                m_HeldKeys.Add((Keys)e.KeyCode);

                if (m_OnKeyDown != null)
                    m_OnKeyDown((Keys)e.KeyCode);
            }
        }
        public void KeyUpHandler(object a_Sender, System.Windows.Forms.KeyEventArgs e)
        {
            m_HeldKeys.Remove((Keys)e.KeyCode);

            if (m_OnKeyUp != null)
                m_OnKeyUp((Keys)e.KeyCode);
        }
#elif UNITY_EDITOR
        private void Start()
        {
            if (!s_Initialized)
            {
                m_OnKeyUp = null;
                m_OnKeyDown = null;

                m_OnMouseDown = null;
                m_OnMouseUp = null;

                m_PrevMousePosition = new Vector2();

                heldKeys = new List<Keys>();
                heldMouseButtons = new List<int>();

                s_Initialized = true;
            }
        }

        private void OnGUI()
        {
            var e = Event.current;

            switch (e.type)
            {
                case EventType.KeyDown:
                    {
                        if (e.keyCode != KeyCode.None && !heldKeys.Contains((Keys)e.keyCode))
                        {
                            heldKeys.Add((Keys)e.keyCode);

                            if (m_OnKeyDown != null)
                                m_OnKeyDown((Keys)e.keyCode);
                        }
                    }
                    break;
                case EventType.KeyUp:
                    {
                        if (e.keyCode != KeyCode.None)
                        {
                            heldKeys.Remove((Keys)e.keyCode);

                            if (m_OnKeyUp != null)
                                m_OnKeyUp((Keys)e.keyCode);
                        }
                    }
                    break;

                case EventType.MouseDown:
                    {
                        heldMouseButtons.Add(e.button);

                        if (m_OnMouseDown != null)
                            m_OnMouseDown(e.button, e.mousePosition);
                    }
                    break;
                case EventType.MouseUp:
                    {
                        heldMouseButtons.Remove(e.button);

                        if (m_OnMouseUp != null)
                            m_OnMouseUp(e.button, e.mousePosition);
                    }
                    break;
            }

            if (m_PrevMousePosition != e.mousePosition)
            {
                if(heldMouseButtons.Count == 0)
                    m_OnMouseMove(-1, e.mousePosition);
                else
                    m_OnMouseMove(e.button, e.mousePosition);

                m_PrevMousePosition = e.mousePosition;
            }
        }

#endif
        public void AddOnKeyDown(OnKeyDelegate a_Delegate)
        {
            m_OnKeyDown += a_Delegate;
        }
        public void AddOnKeyUp(OnKeyDelegate a_Delegate)
        {
            m_OnKeyUp += a_Delegate;
        }

        public void AddOnMouseDown(OnMouseDelegate a_Delegate)
        {
            m_OnMouseDown += a_Delegate;
        }
        public void AddOnMouseUp(OnMouseDelegate a_Delegate)
        {
            m_OnMouseUp += a_Delegate;
        }

        public void AddOnMouseMove(OnMouseDelegate a_Delegate)
        {
            m_OnMouseMove += a_Delegate;
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

    /// <summary>
    /// Very Important part of code. One of these must compile or you can't use this file
    /// </summary>
#if (UNITY_EDITOR && !FORMS)
    /// <summary>
    /// Specifies key codes when using unity
    /// </summary>
    public enum Keys
    {
        //
        // Summary:
        //     ///
        //     Not assigned (never returned as the result of a keystroke).
        //     ///
        None = 0,
        //
        // Summary:
        //     ///
        //     The backspace key.
        //     ///
        Backspace = 8,
        //
        // Summary:
        //     ///
        //     The tab key.
        //     ///
        Tab = 9,
        //
        // Summary:
        //     ///
        //     The Clear key.
        //     ///
        Clear = 12,
        //
        // Summary:
        //     ///
        //     Return key.
        //     ///
        Return = 13,
        //
        // Summary:
        //     ///
        //     Pause on PC machines.
        //     ///
        Pause = 19,
        //
        // Summary:
        //     ///
        //     Escape key.
        //     ///
        Escape = 27,
        //
        // Summary:
        //     ///
        //     Space key.
        //     ///
        Space = 32,
        //
        // Summary:
        //     ///
        //     Exclamation mark key '!'.
        //     ///
        Exclaim = 33,
        //
        // Summary:
        //     ///
        //     Double quote key '"'.
        //     ///
        DoubleQuote = 34,
        //
        // Summary:
        //     ///
        //     Hash key '#'.
        //     ///
        Hash = 35,
        //
        // Summary:
        //     ///
        //     Dollar sign key '$'.
        //     ///
        Dollar = 36,
        //
        // Summary:
        //     ///
        //     Ampersand key '&'.
        //     ///
        Ampersand = 38,
        //
        // Summary:
        //     ///
        //     Quote key '.
        //     ///
        Quote = 39,
        //
        // Summary:
        //     ///
        //     Left Parenthesis key '('.
        //     ///
        LeftParen = 40,
        //
        // Summary:
        //     ///
        //     Right Parenthesis key ')'.
        //     ///
        RightParen = 41,
        //
        // Summary:
        //     ///
        //     Asterisk key '*'.
        //     ///
        Asterisk = 42,
        //
        // Summary:
        //     ///
        //     Plus key '+'.
        //     ///
        Plus = 43,
        //
        // Summary:
        //     ///
        //     Comma ',' key.
        //     ///
        Comma = 44,
        //
        // Summary:
        //     ///
        //     Minus '-' key.
        //     ///
        Minus = 45,
        //
        // Summary:
        //     ///
        //     Period '.' key.
        //     ///
        Period = 46,
        //
        // Summary:
        //     ///
        //     Slash '/' key.
        //     ///
        Slash = 47,
        //
        // Summary:
        //     ///
        //     The '0' key on the top of the alphanumeric keyboard.
        //     ///
        Alpha0 = 48,
        //
        // Summary:
        //     ///
        //     The '1' key on the top of the alphanumeric keyboard.
        //     ///
        Alpha1 = 49,
        //
        // Summary:
        //     ///
        //     The '2' key on the top of the alphanumeric keyboard.
        //     ///
        Alpha2 = 50,
        //
        // Summary:
        //     ///
        //     The '3' key on the top of the alphanumeric keyboard.
        //     ///
        Alpha3 = 51,
        //
        // Summary:
        //     ///
        //     The '4' key on the top of the alphanumeric keyboard.
        //     ///
        Alpha4 = 52,
        //
        // Summary:
        //     ///
        //     The '5' key on the top of the alphanumeric keyboard.
        //     ///
        Alpha5 = 53,
        //
        // Summary:
        //     ///
        //     The '6' key on the top of the alphanumeric keyboard.
        //     ///
        Alpha6 = 54,
        //
        // Summary:
        //     ///
        //     The '7' key on the top of the alphanumeric keyboard.
        //     ///
        Alpha7 = 55,
        //
        // Summary:
        //     ///
        //     The '8' key on the top of the alphanumeric keyboard.
        //     ///
        Alpha8 = 56,
        //
        // Summary:
        //     ///
        //     The '9' key on the top of the alphanumeric keyboard.
        //     ///
        Alpha9 = 57,
        //
        // Summary:
        //     ///
        //     Colon ':' key.
        //     ///
        Colon = 58,
        //
        // Summary:
        //     ///
        //     Semicolon ';' key.
        //     ///
        Semicolon = 59,
        //
        // Summary:
        //     ///
        //     Less than '<' key.
        //     ///
        Less = 60,
        //
        // Summary:
        //     ///
        //     Equals '=' key.
        //     ///
        Equals = 61,
        //
        // Summary:
        //     ///
        //     Greater than '>' key.
        //     ///
        Greater = 62,
        //
        // Summary:
        //     ///
        //     Question mark '?' key.
        //     ///
        Question = 63,
        //
        // Summary:
        //     ///
        //     At key '@'.
        //     ///
        At = 64,
        //
        // Summary:
        //     ///
        //     Left square bracket key '['.
        //     ///
        LeftBracket = 91,
        //
        // Summary:
        //     ///
        //     Backslash key '\'.
        //     ///
        Backslash = 92,
        //
        // Summary:
        //     ///
        //     Right square bracket key ']'.
        //     ///
        RightBracket = 93,
        //
        // Summary:
        //     ///
        //     Caret key '^'.
        //     ///
        Caret = 94,
        //
        // Summary:
        //     ///
        //     Underscore '_' key.
        //     ///
        Underscore = 95,
        //
        // Summary:
        //     ///
        //     Back quote key '`'.
        //     ///
        BackQuote = 96,
        //
        // Summary:
        //     ///
        //     'a' key.
        //     ///
        A = 97,
        //
        // Summary:
        //     ///
        //     'b' key.
        //     ///
        B = 98,
        //
        // Summary:
        //     ///
        //     'c' key.
        //     ///
        C = 99,
        //
        // Summary:
        //     ///
        //     'd' key.
        //     ///
        D = 100,
        //
        // Summary:
        //     ///
        //     'e' key.
        //     ///
        E = 101,
        //
        // Summary:
        //     ///
        //     'f' key.
        //     ///
        F = 102,
        //
        // Summary:
        //     ///
        //     'g' key.
        //     ///
        G = 103,
        //
        // Summary:
        //     ///
        //     'h' key.
        //     ///
        H = 104,
        //
        // Summary:
        //     ///
        //     'i' key.
        //     ///
        I = 105,
        //
        // Summary:
        //     ///
        //     'j' key.
        //     ///
        J = 106,
        //
        // Summary:
        //     ///
        //     'k' key.
        //     ///
        K = 107,
        //
        // Summary:
        //     ///
        //     'l' key.
        //     ///
        L = 108,
        //
        // Summary:
        //     ///
        //     'm' key.
        //     ///
        M = 109,
        //
        // Summary:
        //     ///
        //     'n' key.
        //     ///
        N = 110,
        //
        // Summary:
        //     ///
        //     'o' key.
        //     ///
        O = 111,
        //
        // Summary:
        //     ///
        //     'p' key.
        //     ///
        P = 112,
        //
        // Summary:
        //     ///
        //     'q' key.
        //     ///
        Q = 113,
        //
        // Summary:
        //     ///
        //     'r' key.
        //     ///
        R = 114,
        //
        // Summary:
        //     ///
        //     's' key.
        //     ///
        S = 115,
        //
        // Summary:
        //     ///
        //     't' key.
        //     ///
        T = 116,
        //
        // Summary:
        //     ///
        //     'u' key.
        //     ///
        U = 117,
        //
        // Summary:
        //     ///
        //     'v' key.
        //     ///
        V = 118,
        //
        // Summary:
        //     ///
        //     'w' key.
        //     ///
        W = 119,
        //
        // Summary:
        //     ///
        //     'x' key.
        //     ///
        X = 120,
        //
        // Summary:
        //     ///
        //     'y' key.
        //     ///
        Y = 121,
        //
        // Summary:
        //     ///
        //     'z' key.
        //     ///
        Z = 122,
        //
        // Summary:
        //     ///
        //     The forward delete key.
        //     ///
        Delete = 127,
        //
        // Summary:
        //     ///
        //     Numeric keypad 0.
        //     ///
        NumPad0 = 256,
        //
        // Summary:
        //     ///
        //     Numeric keypad 1.
        //     ///
        NumPad1 = 257,
        //
        // Summary:
        //     ///
        //     Numeric keypad 2.
        //     ///
        NumPad2 = 258,
        //
        // Summary:
        //     ///
        //     Numeric keypad 3.
        //     ///
        NumPad3 = 259,
        //
        // Summary:
        //     ///
        //     Numeric keypad 4.
        //     ///
        NumPad4 = 260,
        //
        // Summary:
        //     ///
        //     Numeric keypad 5.
        //     ///
        NumPad5 = 261,
        //
        // Summary:
        //     ///
        //     Numeric keypad 6.
        //     ///
        NumPad6 = 262,
        //
        // Summary:
        //     ///
        //     Numeric keypad 7.
        //     ///
        NumPad7 = 263,
        //
        // Summary:
        //     ///
        //     Numeric keypad 8.
        //     ///
        NumPad8 = 264,
        //
        // Summary:
        //     ///
        //     Numeric keypad 9.
        //     ///
        NumPad9 = 265,
        //
        // Summary:
        //     ///
        //     Numeric keypad '.'.
        //     ///
        NumPeriod = 266,
        //
        // Summary:
        //     ///
        //     Numeric keypad '/'.
        //     ///
        Divide = 267,
        //
        // Summary:
        //     ///
        //     Numeric keypad '*'.
        //     ///
        Multiply = 268,
        //
        // Summary:
        //     ///
        //     Numeric keypad '-'.
        //     ///
        NumMinus = 269,
        //
        // Summary:
        //     ///
        //     Numeric keypad '+'.
        //     ///
        NumPlus = 270,
        //
        // Summary:
        //     ///
        //     Numeric keypad enter.
        //     ///
        Enter = 271,
        //
        // Summary:
        //     ///
        //     Numeric keypad '='.
        //     ///
        NumEquals = 272,
        //
        // Summary:
        //     ///
        //     Up arrow key.
        //     ///
        Up = 273,
        //
        // Summary:
        //     ///
        //     Down arrow key.
        //     ///
        Down = 274,
        //
        // Summary:
        //     ///
        //     Right arrow key.
        //     ///
        Right = 275,
        //
        // Summary:
        //     ///
        //     Left arrow key.
        //     ///
        Left = 276,
        //
        // Summary:
        //     ///
        //     Insert key key.
        //     ///
        Insert = 277,
        //
        // Summary:
        //     ///
        //     Home key.
        //     ///
        Home = 278,
        //
        // Summary:
        //     ///
        //     End key.
        //     ///
        End = 279,
        //
        // Summary:
        //     ///
        //     Page up.
        //     ///
        PageUp = 280,
        //
        // Summary:
        //     ///
        //     Page down.
        //     ///
        PageDown = 281,
        //
        // Summary:
        //     ///
        //     F1 function key.
        //     ///
        F1 = 282,
        //
        // Summary:
        //     ///
        //     F2 function key.
        //     ///
        F2 = 283,
        //
        // Summary:
        //     ///
        //     F3 function key.
        //     ///
        F3 = 284,
        //
        // Summary:
        //     ///
        //     F4 function key.
        //     ///
        F4 = 285,
        //
        // Summary:
        //     ///
        //     F5 function key.
        //     ///
        F5 = 286,
        //
        // Summary:
        //     ///
        //     F6 function key.
        //     ///
        F6 = 287,
        //
        // Summary:
        //     ///
        //     F7 function key.
        //     ///
        F7 = 288,
        //
        // Summary:
        //     ///
        //     F8 function key.
        //     ///
        F8 = 289,
        //
        // Summary:
        //     ///
        //     F9 function key.
        //     ///
        F9 = 290,
        //
        // Summary:
        //     ///
        //     F10 function key.
        //     ///
        F10 = 291,
        //
        // Summary:
        //     ///
        //     F11 function key.
        //     ///
        F11 = 292,
        //
        // Summary:
        //     ///
        //     F12 function key.
        //     ///
        F12 = 293,
        //
        // Summary:
        //     ///
        //     F13 function key.
        //     ///
        F13 = 294,
        //
        // Summary:
        //     ///
        //     F14 function key.
        //     ///
        F14 = 295,
        //
        // Summary:
        //     ///
        //     F15 function key.
        //     ///
        F15 = 296,
        //
        // Summary:
        //     ///
        //     Numlock key.
        //     ///
        Numlock = 300,
        //
        // Summary:
        //     ///
        //     Capslock key.
        //     ///
        CapsLock = 301,
        //
        // Summary:
        //     ///
        //     Scroll lock key.
        //     ///
        ScrollLock = 302,
        //
        // Summary:
        //     ///
        //     Right shift key.
        //     ///
        RightShift = 303,
        //
        // Summary:
        //     ///
        //     Left shift key.
        //     ///
        LeftShift = 304,
        //
        // Summary:
        //     ///
        //     Right Control key.
        //     ///
        RightControl = 305,
        //
        // Summary:
        //     ///
        //     Left Control key.
        //     ///
        LeftControl = 306,
        //
        // Summary:
        //     ///
        //     Right Alt key.
        //     ///
        RightAlt = 307,
        //
        // Summary:
        //     ///
        //     Left Alt key.
        //     ///
        LeftAlt = 308,
        //
        // Summary:
        //     ///
        //     Right Command key.
        //     ///
        RightCommand = 309,
        //
        // Summary:
        //     ///
        //     Right Command key.
        //     ///
        RightApple = 309,
        //
        // Summary:
        //     ///
        //     Left Command key.
        //     ///
        LeftCommand = 310,
        //
        // Summary:
        //     ///
        //     Left Command key.
        //     ///
        LeftApple = 310,
        //
        // Summary:
        //     ///
        //     Left Windows key.
        //     ///
        LeftWindows = 311,
        //
        // Summary:
        //     ///
        //     Right Windows key.
        //     ///
        RightWindows = 312,
        //
        // Summary:
        //     ///
        //     Alt Gr key.
        //     ///
        AltGr = 313,
        //
        // Summary:
        //     ///
        //     Help key.
        //     ///
        Help = 315,
        //
        // Summary:
        //     ///
        //     Print key.
        //     ///
        Print = 316,
        //
        // Summary:
        //     ///
        //     Sys Req key.
        //     ///
        SysReq = 317,
        //
        // Summary:
        //     ///
        //     Break key.
        //     ///
        Break = 318,
        //
        // Summary:
        //     ///
        //     Menu key.
        //     ///
        Menu = 319,
        //
        // Summary:
        //     ///
        //     First (primary) mouse button.
        //     ///
        Mouse0 = 323,
        //
        // Summary:
        //     ///
        //     Second (secondary) mouse button.
        //     ///
        Mouse1 = 324,
        //
        // Summary:
        //     ///
        //     Third mouse button.
        //     ///
        Mouse2 = 325,
        //
        // Summary:
        //     ///
        //     Fourth mouse button.
        //     ///
        Mouse3 = 326,
        //
        // Summary:
        //     ///
        //     Fifth mouse button.
        //     ///
        Mouse4 = 327,
        //
        // Summary:
        //     ///
        //     Sixth mouse button.
        //     ///
        Mouse5 = 328,
        //
        // Summary:
        //     ///
        //     Seventh mouse button.
        //     ///
        Mouse6 = 329,
        //
        // Summary:
        //     ///
        //     Button 0 on any joystick.
        //     ///
        JoystickButton0 = 330,
        //
        // Summary:
        //     ///
        //     Button 1 on any joystick.
        //     ///
        JoystickButton1 = 331,
        //
        // Summary:
        //     ///
        //     Button 2 on any joystick.
        //     ///
        JoystickButton2 = 332,
        //
        // Summary:
        //     ///
        //     Button 3 on any joystick.
        //     ///
        JoystickButton3 = 333,
        //
        // Summary:
        //     ///
        //     Button 4 on any joystick.
        //     ///
        JoystickButton4 = 334,
        //
        // Summary:
        //     ///
        //     Button 5 on any joystick.
        //     ///
        JoystickButton5 = 335,
        //
        // Summary:
        //     ///
        //     Button 6 on any joystick.
        //     ///
        JoystickButton6 = 336,
        //
        // Summary:
        //     ///
        //     Button 7 on any joystick.
        //     ///
        JoystickButton7 = 337,
        //
        // Summary:
        //     ///
        //     Button 8 on any joystick.
        //     ///
        JoystickButton8 = 338,
        //
        // Summary:
        //     ///
        //     Button 9 on any joystick.
        //     ///
        JoystickButton9 = 339,
        //
        // Summary:
        //     ///
        //     Button 10 on any joystick.
        //     ///
        JoystickButton10 = 340,
        //
        // Summary:
        //     ///
        //     Button 11 on any joystick.
        //     ///
        JoystickButton11 = 341,
        //
        // Summary:
        //     ///
        //     Button 12 on any joystick.
        //     ///
        JoystickButton12 = 342,
        //
        // Summary:
        //     ///
        //     Button 13 on any joystick.
        //     ///
        JoystickButton13 = 343,
        //
        // Summary:
        //     ///
        //     Button 14 on any joystick.
        //     ///
        JoystickButton14 = 344,
        //
        // Summary:
        //     ///
        //     Button 15 on any joystick.
        //     ///
        JoystickButton15 = 345,
        //
        // Summary:
        //     ///
        //     Button 16 on any joystick.
        //     ///
        JoystickButton16 = 346,
        //
        // Summary:
        //     ///
        //     Button 17 on any joystick.
        //     ///
        JoystickButton17 = 347,
        //
        // Summary:
        //     ///
        //     Button 18 on any joystick.
        //     ///
        JoystickButton18 = 348,
        //
        // Summary:
        //     ///
        //     Button 19 on any joystick.
        //     ///
        JoystickButton19 = 349,
        //
        // Summary:
        //     ///
        //     Button 0 on first joystick.
        //     ///
        Joystick1Button0 = 350,
        //
        // Summary:
        //     ///
        //     Button 1 on first joystick.
        //     ///
        Joystick1Button1 = 351,
        //
        // Summary:
        //     ///
        //     Button 2 on first joystick.
        //     ///
        Joystick1Button2 = 352,
        //
        // Summary:
        //     ///
        //     Button 3 on first joystick.
        //     ///
        Joystick1Button3 = 353,
        //
        // Summary:
        //     ///
        //     Button 4 on first joystick.
        //     ///
        Joystick1Button4 = 354,
        //
        // Summary:
        //     ///
        //     Button 5 on first joystick.
        //     ///
        Joystick1Button5 = 355,
        //
        // Summary:
        //     ///
        //     Button 6 on first joystick.
        //     ///
        Joystick1Button6 = 356,
        //
        // Summary:
        //     ///
        //     Button 7 on first joystick.
        //     ///
        Joystick1Button7 = 357,
        //
        // Summary:
        //     ///
        //     Button 8 on first joystick.
        //     ///
        Joystick1Button8 = 358,
        //
        // Summary:
        //     ///
        //     Button 9 on first joystick.
        //     ///
        Joystick1Button9 = 359,
        //
        // Summary:
        //     ///
        //     Button 10 on first joystick.
        //     ///
        Joystick1Button10 = 360,
        //
        // Summary:
        //     ///
        //     Button 11 on first joystick.
        //     ///
        Joystick1Button11 = 361,
        //
        // Summary:
        //     ///
        //     Button 12 on first joystick.
        //     ///
        Joystick1Button12 = 362,
        //
        // Summary:
        //     ///
        //     Button 13 on first joystick.
        //     ///
        Joystick1Button13 = 363,
        //
        // Summary:
        //     ///
        //     Button 14 on first joystick.
        //     ///
        Joystick1Button14 = 364,
        //
        // Summary:
        //     ///
        //     Button 15 on first joystick.
        //     ///
        Joystick1Button15 = 365,
        //
        // Summary:
        //     ///
        //     Button 16 on first joystick.
        //     ///
        Joystick1Button16 = 366,
        //
        // Summary:
        //     ///
        //     Button 17 on first joystick.
        //     ///
        Joystick1Button17 = 367,
        //
        // Summary:
        //     ///
        //     Button 18 on first joystick.
        //     ///
        Joystick1Button18 = 368,
        //
        // Summary:
        //     ///
        //     Button 19 on first joystick.
        //     ///
        Joystick1Button19 = 369,
        //
        // Summary:
        //     ///
        //     Button 0 on second joystick.
        //     ///
        Joystick2Button0 = 370,
        //
        // Summary:
        //     ///
        //     Button 1 on second joystick.
        //     ///
        Joystick2Button1 = 371,
        //
        // Summary:
        //     ///
        //     Button 2 on second joystick.
        //     ///
        Joystick2Button2 = 372,
        //
        // Summary:
        //     ///
        //     Button 3 on second joystick.
        //     ///
        Joystick2Button3 = 373,
        //
        // Summary:
        //     ///
        //     Button 4 on second joystick.
        //     ///
        Joystick2Button4 = 374,
        //
        // Summary:
        //     ///
        //     Button 5 on second joystick.
        //     ///
        Joystick2Button5 = 375,
        //
        // Summary:
        //     ///
        //     Button 6 on second joystick.
        //     ///
        Joystick2Button6 = 376,
        //
        // Summary:
        //     ///
        //     Button 7 on second joystick.
        //     ///
        Joystick2Button7 = 377,
        //
        // Summary:
        //     ///
        //     Button 8 on second joystick.
        //     ///
        Joystick2Button8 = 378,
        //
        // Summary:
        //     ///
        //     Button 9 on second joystick.
        //     ///
        Joystick2Button9 = 379,
        //
        // Summary:
        //     ///
        //     Button 10 on second joystick.
        //     ///
        Joystick2Button10 = 380,
        //
        // Summary:
        //     ///
        //     Button 11 on second joystick.
        //     ///
        Joystick2Button11 = 381,
        //
        // Summary:
        //     ///
        //     Button 12 on second joystick.
        //     ///
        Joystick2Button12 = 382,
        //
        // Summary:
        //     ///
        //     Button 13 on second joystick.
        //     ///
        Joystick2Button13 = 383,
        //
        // Summary:
        //     ///
        //     Button 14 on second joystick.
        //     ///
        Joystick2Button14 = 384,
        //
        // Summary:
        //     ///
        //     Button 15 on second joystick.
        //     ///
        Joystick2Button15 = 385,
        //
        // Summary:
        //     ///
        //     Button 16 on second joystick.
        //     ///
        Joystick2Button16 = 386,
        //
        // Summary:
        //     ///
        //     Button 17 on second joystick.
        //     ///
        Joystick2Button17 = 387,
        //
        // Summary:
        //     ///
        //     Button 18 on second joystick.
        //     ///
        Joystick2Button18 = 388,
        //
        // Summary:
        //     ///
        //     Button 19 on second joystick.
        //     ///
        Joystick2Button19 = 389,
        //
        // Summary:
        //     ///
        //     Button 0 on third joystick.
        //     ///
        Joystick3Button0 = 390,
        //
        // Summary:
        //     ///
        //     Button 1 on third joystick.
        //     ///
        Joystick3Button1 = 391,
        //
        // Summary:
        //     ///
        //     Button 2 on third joystick.
        //     ///
        Joystick3Button2 = 392,
        //
        // Summary:
        //     ///
        //     Button 3 on third joystick.
        //     ///
        Joystick3Button3 = 393,
        //
        // Summary:
        //     ///
        //     Button 4 on third joystick.
        //     ///
        Joystick3Button4 = 394,
        //
        // Summary:
        //     ///
        //     Button 5 on third joystick.
        //     ///
        Joystick3Button5 = 395,
        //
        // Summary:
        //     ///
        //     Button 6 on third joystick.
        //     ///
        Joystick3Button6 = 396,
        //
        // Summary:
        //     ///
        //     Button 7 on third joystick.
        //     ///
        Joystick3Button7 = 397,
        //
        // Summary:
        //     ///
        //     Button 8 on third joystick.
        //     ///
        Joystick3Button8 = 398,
        //
        // Summary:
        //     ///
        //     Button 9 on third joystick.
        //     ///
        Joystick3Button9 = 399,
        //
        // Summary:
        //     ///
        //     Button 10 on third joystick.
        //     ///
        Joystick3Button10 = 400,
        //
        // Summary:
        //     ///
        //     Button 11 on third joystick.
        //     ///
        Joystick3Button11 = 401,
        //
        // Summary:
        //     ///
        //     Button 12 on third joystick.
        //     ///
        Joystick3Button12 = 402,
        //
        // Summary:
        //     ///
        //     Button 13 on third joystick.
        //     ///
        Joystick3Button13 = 403,
        //
        // Summary:
        //     ///
        //     Button 14 on third joystick.
        //     ///
        Joystick3Button14 = 404,
        //
        // Summary:
        //     ///
        //     Button 15 on third joystick.
        //     ///
        Joystick3Button15 = 405,
        //
        // Summary:
        //     ///
        //     Button 16 on third joystick.
        //     ///
        Joystick3Button16 = 406,
        //
        // Summary:
        //     ///
        //     Button 17 on third joystick.
        //     ///
        Joystick3Button17 = 407,
        //
        // Summary:
        //     ///
        //     Button 18 on third joystick.
        //     ///
        Joystick3Button18 = 408,
        //
        // Summary:
        //     ///
        //     Button 19 on third joystick.
        //     ///
        Joystick3Button19 = 409,
        //
        // Summary:
        //     ///
        //     Button 0 on forth joystick.
        //     ///
        Joystick4Button0 = 410,
        //
        // Summary:
        //     ///
        //     Button 1 on forth joystick.
        //     ///
        Joystick4Button1 = 411,
        //
        // Summary:
        //     ///
        //     Button 2 on forth joystick.
        //     ///
        Joystick4Button2 = 412,
        //
        // Summary:
        //     ///
        //     Button 3 on forth joystick.
        //     ///
        Joystick4Button3 = 413,
        //
        // Summary:
        //     ///
        //     Button 4 on forth joystick.
        //     ///
        Joystick4Button4 = 414,
        //
        // Summary:
        //     ///
        //     Button 5 on forth joystick.
        //     ///
        Joystick4Button5 = 415,
        //
        // Summary:
        //     ///
        //     Button 6 on forth joystick.
        //     ///
        Joystick4Button6 = 416,
        //
        // Summary:
        //     ///
        //     Button 7 on forth joystick.
        //     ///
        Joystick4Button7 = 417,
        //
        // Summary:
        //     ///
        //     Button 8 on forth joystick.
        //     ///
        Joystick4Button8 = 418,
        //
        // Summary:
        //     ///
        //     Button 9 on forth joystick.
        //     ///
        Joystick4Button9 = 419,
        //
        // Summary:
        //     ///
        //     Button 10 on forth joystick.
        //     ///
        Joystick4Button10 = 420,
        //
        // Summary:
        //     ///
        //     Button 11 on forth joystick.
        //     ///
        Joystick4Button11 = 421,
        //
        // Summary:
        //     ///
        //     Button 12 on forth joystick.
        //     ///
        Joystick4Button12 = 422,
        //
        // Summary:
        //     ///
        //     Button 13 on forth joystick.
        //     ///
        Joystick4Button13 = 423,
        //
        // Summary:
        //     ///
        //     Button 14 on forth joystick.
        //     ///
        Joystick4Button14 = 424,
        //
        // Summary:
        //     ///
        //     Button 15 on forth joystick.
        //     ///
        Joystick4Button15 = 425,
        //
        // Summary:
        //     ///
        //     Button 16 on forth joystick.
        //     ///
        Joystick4Button16 = 426,
        //
        // Summary:
        //     ///
        //     Button 17 on forth joystick.
        //     ///
        Joystick4Button17 = 427,
        //
        // Summary:
        //     ///
        //     Button 18 on forth joystick.
        //     ///
        Joystick4Button18 = 428,
        //
        // Summary:
        //     ///
        //     Button 19 on forth joystick.
        //     ///
        Joystick4Button19 = 429,
        //
        // Summary:
        //     ///
        //     Button 0 on fifth joystick.
        //     ///
        Joystick5Button0 = 430,
        //
        // Summary:
        //     ///
        //     Button 1 on fifth joystick.
        //     ///
        Joystick5Button1 = 431,
        //
        // Summary:
        //     ///
        //     Button 2 on fifth joystick.
        //     ///
        Joystick5Button2 = 432,
        //
        // Summary:
        //     ///
        //     Button 3 on fifth joystick.
        //     ///
        Joystick5Button3 = 433,
        //
        // Summary:
        //     ///
        //     Button 4 on fifth joystick.
        //     ///
        Joystick5Button4 = 434,
        //
        // Summary:
        //     ///
        //     Button 5 on fifth joystick.
        //     ///
        Joystick5Button5 = 435,
        //
        // Summary:
        //     ///
        //     Button 6 on fifth joystick.
        //     ///
        Joystick5Button6 = 436,
        //
        // Summary:
        //     ///
        //     Button 7 on fifth joystick.
        //     ///
        Joystick5Button7 = 437,
        //
        // Summary:
        //     ///
        //     Button 8 on fifth joystick.
        //     ///
        Joystick5Button8 = 438,
        //
        // Summary:
        //     ///
        //     Button 9 on fifth joystick.
        //     ///
        Joystick5Button9 = 439,
        //
        // Summary:
        //     ///
        //     Button 10 on fifth joystick.
        //     ///
        Joystick5Button10 = 440,
        //
        // Summary:
        //     ///
        //     Button 11 on fifth joystick.
        //     ///
        Joystick5Button11 = 441,
        //
        // Summary:
        //     ///
        //     Button 12 on fifth joystick.
        //     ///
        Joystick5Button12 = 442,
        //
        // Summary:
        //     ///
        //     Button 13 on fifth joystick.
        //     ///
        Joystick5Button13 = 443,
        //
        // Summary:
        //     ///
        //     Button 14 on fifth joystick.
        //     ///
        Joystick5Button14 = 444,
        //
        // Summary:
        //     ///
        //     Button 15 on fifth joystick.
        //     ///
        Joystick5Button15 = 445,
        //
        // Summary:
        //     ///
        //     Button 16 on fifth joystick.
        //     ///
        Joystick5Button16 = 446,
        //
        // Summary:
        //     ///
        //     Button 17 on fifth joystick.
        //     ///
        Joystick5Button17 = 447,
        //
        // Summary:
        //     ///
        //     Button 18 on fifth joystick.
        //     ///
        Joystick5Button18 = 448,
        //
        // Summary:
        //     ///
        //     Button 19 on fifth joystick.
        //     ///
        Joystick5Button19 = 449,
        //
        // Summary:
        //     ///
        //     Button 0 on sixth joystick.
        //     ///
        Joystick6Button0 = 450,
        //
        // Summary:
        //     ///
        //     Button 1 on sixth joystick.
        //     ///
        Joystick6Button1 = 451,
        //
        // Summary:
        //     ///
        //     Button 2 on sixth joystick.
        //     ///
        Joystick6Button2 = 452,
        //
        // Summary:
        //     ///
        //     Button 3 on sixth joystick.
        //     ///
        Joystick6Button3 = 453,
        //
        // Summary:
        //     ///
        //     Button 4 on sixth joystick.
        //     ///
        Joystick6Button4 = 454,
        //
        // Summary:
        //     ///
        //     Button 5 on sixth joystick.
        //     ///
        Joystick6Button5 = 455,
        //
        // Summary:
        //     ///
        //     Button 6 on sixth joystick.
        //     ///
        Joystick6Button6 = 456,
        //
        // Summary:
        //     ///
        //     Button 7 on sixth joystick.
        //     ///
        Joystick6Button7 = 457,
        //
        // Summary:
        //     ///
        //     Button 8 on sixth joystick.
        //     ///
        Joystick6Button8 = 458,
        //
        // Summary:
        //     ///
        //     Button 9 on sixth joystick.
        //     ///
        Joystick6Button9 = 459,
        //
        // Summary:
        //     ///
        //     Button 10 on sixth joystick.
        //     ///
        Joystick6Button10 = 460,
        //
        // Summary:
        //     ///
        //     Button 11 on sixth joystick.
        //     ///
        Joystick6Button11 = 461,
        //
        // Summary:
        //     ///
        //     Button 12 on sixth joystick.
        //     ///
        Joystick6Button12 = 462,
        //
        // Summary:
        //     ///
        //     Button 13 on sixth joystick.
        //     ///
        Joystick6Button13 = 463,
        //
        // Summary:
        //     ///
        //     Button 14 on sixth joystick.
        //     ///
        Joystick6Button14 = 464,
        //
        // Summary:
        //     ///
        //     Button 15 on sixth joystick.
        //     ///
        Joystick6Button15 = 465,
        //
        // Summary:
        //     ///
        //     Button 16 on sixth joystick.
        //     ///
        Joystick6Button16 = 466,
        //
        // Summary:
        //     ///
        //     Button 17 on sixth joystick.
        //     ///
        Joystick6Button17 = 467,
        //
        // Summary:
        //     ///
        //     Button 18 on sixth joystick.
        //     ///
        Joystick6Button18 = 468,
        //
        // Summary:
        //     ///
        //     Button 19 on sixth joystick.
        //     ///
        Joystick6Button19 = 469,
        //
        // Summary:
        //     ///
        //     Button 0 on seventh joystick.
        //     ///
        Joystick7Button0 = 470,
        //
        // Summary:
        //     ///
        //     Button 1 on seventh joystick.
        //     ///
        Joystick7Button1 = 471,
        //
        // Summary:
        //     ///
        //     Button 2 on seventh joystick.
        //     ///
        Joystick7Button2 = 472,
        //
        // Summary:
        //     ///
        //     Button 3 on seventh joystick.
        //     ///
        Joystick7Button3 = 473,
        //
        // Summary:
        //     ///
        //     Button 4 on seventh joystick.
        //     ///
        Joystick7Button4 = 474,
        //
        // Summary:
        //     ///
        //     Button 5 on seventh joystick.
        //     ///
        Joystick7Button5 = 475,
        //
        // Summary:
        //     ///
        //     Button 6 on seventh joystick.
        //     ///
        Joystick7Button6 = 476,
        //
        // Summary:
        //     ///
        //     Button 7 on seventh joystick.
        //     ///
        Joystick7Button7 = 477,
        //
        // Summary:
        //     ///
        //     Button 8 on seventh joystick.
        //     ///
        Joystick7Button8 = 478,
        //
        // Summary:
        //     ///
        //     Button 9 on seventh joystick.
        //     ///
        Joystick7Button9 = 479,
        //
        // Summary:
        //     ///
        //     Button 10 on seventh joystick.
        //     ///
        Joystick7Button10 = 480,
        //
        // Summary:
        //     ///
        //     Button 11 on seventh joystick.
        //     ///
        Joystick7Button11 = 481,
        //
        // Summary:
        //     ///
        //     Button 12 on seventh joystick.
        //     ///
        Joystick7Button12 = 482,
        //
        // Summary:
        //     ///
        //     Button 13 on seventh joystick.
        //     ///
        Joystick7Button13 = 483,
        //
        // Summary:
        //     ///
        //     Button 14 on seventh joystick.
        //     ///
        Joystick7Button14 = 484,
        //
        // Summary:
        //     ///
        //     Button 15 on seventh joystick.
        //     ///
        Joystick7Button15 = 485,
        //
        // Summary:
        //     ///
        //     Button 16 on seventh joystick.
        //     ///
        Joystick7Button16 = 486,
        //
        // Summary:
        //     ///
        //     Button 17 on seventh joystick.
        //     ///
        Joystick7Button17 = 487,
        //
        // Summary:
        //     ///
        //     Button 18 on seventh joystick.
        //     ///
        Joystick7Button18 = 488,
        //
        // Summary:
        //     ///
        //     Button 19 on seventh joystick.
        //     ///
        Joystick7Button19 = 489,
        //
        // Summary:
        //     ///
        //     Button 0 on eighth joystick.
        //     ///
        Joystick8Button0 = 490,
        //
        // Summary:
        //     ///
        //     Button 1 on eighth joystick.
        //     ///
        Joystick8Button1 = 491,
        //
        // Summary:
        //     ///
        //     Button 2 on eighth joystick.
        //     ///
        Joystick8Button2 = 492,
        //
        // Summary:
        //     ///
        //     Button 3 on eighth joystick.
        //     ///
        Joystick8Button3 = 493,
        //
        // Summary:
        //     ///
        //     Button 4 on eighth joystick.
        //     ///
        Joystick8Button4 = 494,
        //
        // Summary:
        //     ///
        //     Button 5 on eighth joystick.
        //     ///
        Joystick8Button5 = 495,
        //
        // Summary:
        //     ///
        //     Button 6 on eighth joystick.
        //     ///
        Joystick8Button6 = 496,
        //
        // Summary:
        //     ///
        //     Button 7 on eighth joystick.
        //     ///
        Joystick8Button7 = 497,
        //
        // Summary:
        //     ///
        //     Button 8 on eighth joystick.
        //     ///
        Joystick8Button8 = 498,
        //
        // Summary:
        //     ///
        //     Button 9 on eighth joystick.
        //     ///
        Joystick8Button9 = 499,
        //
        // Summary:
        //     ///
        //     Button 10 on eighth joystick.
        //     ///
        Joystick8Button10 = 500,
        //
        // Summary:
        //     ///
        //     Button 11 on eighth joystick.
        //     ///
        Joystick8Button11 = 501,
        //
        // Summary:
        //     ///
        //     Button 12 on eighth joystick.
        //     ///
        Joystick8Button12 = 502,
        //
        // Summary:
        //     ///
        //     Button 13 on eighth joystick.
        //     ///
        Joystick8Button13 = 503,
        //
        // Summary:
        //     ///
        //     Button 14 on eighth joystick.
        //     ///
        Joystick8Button14 = 504,
        //
        // Summary:
        //     ///
        //     Button 15 on eighth joystick.
        //     ///
        Joystick8Button15 = 505,
        //
        // Summary:
        //     ///
        //     Button 16 on eighth joystick.
        //     ///
        Joystick8Button16 = 506,
        //
        // Summary:
        //     ///
        //     Button 17 on eighth joystick.
        //     ///
        Joystick8Button17 = 507,
        //
        // Summary:
        //     ///
        //     Button 18 on eighth joystick.
        //     ///
        Joystick8Button18 = 508,
        //
        // Summary:
        //     ///
        //     Button 19 on eighth joystick.
        //     ///
        Joystick8Button19 = 509
    }

#elif FORMS
        /// <summary>
        /// Specifies key codes when using windows forms
        /// </summary>
        public enum Keys
        {
            //
            // Summary:
            //     The bitmask to extract modifiers from a key value.
            Modifiers = -65536,
            //
            // Summary:
            //     No key pressed.
            None = 0,
            //
            // Summary:
            //     The left mouse button.
            LButton = 1,
            //
            // Summary:
            //     The right mouse button.
            RButton = 2,
            //
            // Summary:
            //     The CANCEL key.
            Cancel = 3,
            //
            // Summary:
            //     The middle mouse button (three-button mouse).
            MButton = 4,
            //
            // Summary:
            //     The first x mouse button (five-button mouse).
            XButton1 = 5,
            //
            // Summary:
            //     The second x mouse button (five-button mouse).
            XButton2 = 6,
            //
            // Summary:
            //     The BACKSPACE key.
            Backspace = 8,
            //
            // Summary:
            //     The TAB key.
            Tab = 9,
            //
            // Summary:
            //     The LINEFEED key.
            LineFeed = 10,
            //
            // Summary:
            //     The CLEAR key.
            Clear = 12,
            //
            // Summary:
            //     The RETURN key.
            Return = 13,
            //
            // Summary:
            //     The ENTER key.
            Enter = 13,
            //
            // Summary:
            //     The SHIFT key.
            ShiftKey = 16,
            //
            // Summary:
            //     The CTRL key.
            ControlKey = 17,
            //
            // Summary:
            //     The ALT key.
            Menu = 18,
            //
            // Summary:
            //     The PAUSE key.
            Pause = 19,
            //
            // Summary:
            //     The CAPS LOCK key.
            Capital = 20,
            //
            // Summary:
            //     The CAPS LOCK key.
            CapsLock = 20,
            //
            // Summary:
            //     The IME Kana mode key.
            KanaMode = 21,
            //
            // Summary:
            //     The IME Hanguel mode key. (maintained for compatibility; use HangulMode)
            HanguelMode = 21,
            //
            // Summary:
            //     The IME Hangul mode key.
            HangulMode = 21,
            //
            // Summary:
            //     The IME Junja mode key.
            JunjaMode = 23,
            //
            // Summary:
            //     The IME final mode key.
            FinalMode = 24,
            //
            // Summary:
            //     The IME Hanja mode key.
            HanjaMode = 25,
            //
            // Summary:
            //     The IME Kanji mode key.
            KanjiMode = 25,
            //
            // Summary:
            //     The ESC key.
            Escape = 27,
            //
            // Summary:
            //     The IME convert key.
            IMEConvert = 28,
            //
            // Summary:
            //     The IME nonconvert key.
            IMENonconvert = 29,
            //
            // Summary:
            //     The IME accept key, replaces System.Windows.Forms.Keys.IMEAceept.
            IMEAccept = 30,
            //
            // Summary:
            //     The IME accept key. Obsolete, use System.Windows.Forms.Keys.IMEAccept instead.
            IMEAceept = 30,
            //
            // Summary:
            //     The IME mode change key.
            IMEModeChange = 31,
            //
            // Summary:
            //     The SPACEBAR key.
            Space = 32,
            //
            // Summary:
            //     The PAGE UP key.
            Prior = 33,
            //
            // Summary:
            //     The PAGE UP key.
            PageUp = 33,
            //
            // Summary:
            //     The PAGE DOWN key.
            Next = 34,
            //
            // Summary:
            //     The PAGE DOWN key.
            PageDown = 34,
            //
            // Summary:
            //     The END key.
            End = 35,
            //
            // Summary:
            //     The HOME key.
            Home = 36,
            //
            // Summary:
            //     The LEFT ARROW key.
            Left = 37,
            //
            // Summary:
            //     The UP ARROW key.
            Up = 38,
            //
            // Summary:
            //     The RIGHT ARROW key.
            Right = 39,
            //
            // Summary:
            //     The DOWN ARROW key.
            Down = 40,
            //
            // Summary:
            //     The SELECT key.
            Select = 41,
            //
            // Summary:
            //     The PRINT key.
            Print = 42,
            //
            // Summary:
            //     The EXECUTE key.
            Execute = 43,
            //
            // Summary:
            //     The PRINT SCREEN key.
            Snapshot = 44,
            //
            // Summary:
            //     The PRINT SCREEN key.
            PrintScreen = 44,
            //
            // Summary:
            //     The INS key.
            Insert = 45,
            //
            // Summary:
            //     The DEL key.
            Delete = 46,
            //
            // Summary:
            //     The HELP key.
            Help = 47,
            //
            // Summary:
            //     The 0 key.
            Alpha0 = 48,
            //
            // Summary:
            //     The 1 key.
            Alpha1 = 49,
            //
            // Summary:
            //     The 2 key.
            Alpha2 = 50,
            //
            // Summary:
            //     The 3 key.
            Alpha3 = 51,
            //
            // Summary:
            //     The 4 key.
            Alpha4 = 52,
            //
            // Summary:
            //     The 5 key.
            Alpha5 = 53,
            //
            // Summary:
            //     The 6 key.
            Alpha6 = 54,
            //
            // Summary:
            //     The 7 key.
            Alpha7 = 55,
            //
            // Summary:
            //     The 8 key.
            Alpha8 = 56,
            //
            // Summary:
            //     The 9 key.
            Alpha9 = 57,
            //
            // Summary:
            //     The A key.
            A = 65,
            //
            // Summary:
            //     The B key.
            B = 66,
            //
            // Summary:
            //     The C key.
            C = 67,
            //
            // Summary:
            //     The D key.
            D = 68,
            //
            // Summary:
            //     The E key.
            E = 69,
            //
            // Summary:
            //     The F key.
            F = 70,
            //
            // Summary:
            //     The G key.
            G = 71,
            //
            // Summary:
            //     The H key.
            H = 72,
            //
            // Summary:
            //     The I key.
            I = 73,
            //
            // Summary:
            //     The J key.
            J = 74,
            //
            // Summary:
            //     The K key.
            K = 75,
            //
            // Summary:
            //     The L key.
            L = 76,
            //
            // Summary:
            //     The M key.
            M = 77,
            //
            // Summary:
            //     The N key.
            N = 78,
            //
            // Summary:
            //     The O key.
            O = 79,
            //
            // Summary:
            //     The P key.
            P = 80,
            //
            // Summary:
            //     The Q key.
            Q = 81,
            //
            // Summary:
            //     The R key.
            R = 82,
            //
            // Summary:
            //     The S key.
            S = 83,
            //
            // Summary:
            //     The T key.
            T = 84,
            //
            // Summary:
            //     The U key.
            U = 85,
            //
            // Summary:
            //     The V key.
            V = 86,
            //
            // Summary:
            //     The W key.
            W = 87,
            //
            // Summary:
            //     The X key.
            X = 88,
            //
            // Summary:
            //     The Y key.
            Y = 89,
            //
            // Summary:
            //     The Z key.
            Z = 90,
            //
            // Summary:
            //     The left Windows logo key (Microsoft Natural Keyboard).
            LeftWindows = 91,
            //
            // Summary:
            //     The right Windows logo key (Microsoft Natural Keyboard).
            RightWindows = 92,
            //
            // Summary:
            //     The application key (Microsoft Natural Keyboard).
            Apps = 93,
            //
            // Summary:
            //     The computer sleep key.
            Sleep = 95,
            //
            // Summary:
            //     The 0 key on the numeric keypad.
            NumPad0 = 96,
            //
            // Summary:
            //     The 1 key on the numeric keypad.
            NumPad1 = 97,
            //
            // Summary:
            //     The 2 key on the numeric keypad.
            NumPad2 = 98,
            //
            // Summary:
            //     The 3 key on the numeric keypad.
            NumPad3 = 99,
            //
            // Summary:
            //     The 4 key on the numeric keypad.
            NumPad4 = 100,
            //
            // Summary:
            //     The 5 key on the numeric keypad.
            NumPad5 = 101,
            //
            // Summary:
            //     The 6 key on the numeric keypad.
            NumPad6 = 102,
            //
            // Summary:
            //     The 7 key on the numeric keypad.
            NumPad7 = 103,
            //
            // Summary:
            //     The 8 key on the numeric keypad.
            NumPad8 = 104,
            //
            // Summary:
            //     The 9 key on the numeric keypad.
            NumPad9 = 105,
            //
            // Summary:
            //     The multiply key.
            Multiply = 106,
            //
            // Summary:
            //     The add key.
            Add = 107,
            //
            // Summary:
            //     The separator key.
            Separator = 108,
            //
            // Summary:
            //     The subtract key.
            Subtract = 109,
            //
            // Summary:
            //     The decimal key.
            Decimal = 110,
            //
            // Summary:
            //     The divide key.
            Divide = 111,
            //
            // Summary:
            //     The F1 key.
            F1 = 112,
            //
            // Summary:
            //     The F2 key.
            F2 = 113,
            //
            // Summary:
            //     The F3 key.
            F3 = 114,
            //
            // Summary:
            //     The F4 key.
            F4 = 115,
            //
            // Summary:
            //     The F5 key.
            F5 = 116,
            //
            // Summary:
            //     The F6 key.
            F6 = 117,
            //
            // Summary:
            //     The F7 key.
            F7 = 118,
            //
            // Summary:
            //     The F8 key.
            F8 = 119,
            //
            // Summary:
            //     The F9 key.
            F9 = 120,
            //
            // Summary:
            //     The F10 key.
            F10 = 121,
            //
            // Summary:
            //     The F11 key.
            F11 = 122,
            //
            // Summary:
            //     The F12 key.
            F12 = 123,
            //
            // Summary:
            //     The F13 key.
            F13 = 124,
            //
            // Summary:
            //     The F14 key.
            F14 = 125,
            //
            // Summary:
            //     The F15 key.
            F15 = 126,
            //
            // Summary:
            //     The F16 key.
            F16 = 127,
            //
            // Summary:
            //     The F17 key.
            F17 = 128,
            //
            // Summary:
            //     The F18 key.
            F18 = 129,
            //
            // Summary:
            //     The F19 key.
            F19 = 130,
            //
            // Summary:
            //     The F20 key.
            F20 = 131,
            //
            // Summary:
            //     The F21 key.
            F21 = 132,
            //
            // Summary:
            //     The F22 key.
            F22 = 133,
            //
            // Summary:
            //     The F23 key.
            F23 = 134,
            //
            // Summary:
            //     The F24 key.
            F24 = 135,
            //
            // Summary:
            //     The NUM LOCK key.
            NumLock = 144,
            //
            // Summary:
            //     The SCROLL LOCK key.
            Scroll = 145,
            //
            // Summary:
            //     The left SHIFT key.
            LeftShift = 160,
            //
            // Summary:
            //     The right SHIFT key.
            RightShift = 161,
            //
            // Summary:
            //     The left CTRL key.
            LeftControl = 162,
            //
            // Summary:
            //     The right CTRL key.
            RightControl = 163,
            //
            // Summary:
            //     The left ALT key.
            LeftAlt = 164,
            //
            // Summary:
            //     The right ALT key.
            RightAlt = 165,
            //
            // Summary:
            //     The browser back key (Windows 2000 or later).
            BrowserBack = 166,
            //
            // Summary:
            //     The browser forward key (Windows 2000 or later).
            BrowserForward = 167,
            //
            // Summary:
            //     The browser refresh key (Windows 2000 or later).
            BrowserRefresh = 168,
            //
            // Summary:
            //     The browser stop key (Windows 2000 or later).
            BrowserStop = 169,
            //
            // Summary:
            //     The browser search key (Windows 2000 or later).
            BrowserSearch = 170,
            //
            // Summary:
            //     The browser favorites key (Windows 2000 or later).
            BrowserFavorites = 171,
            //
            // Summary:
            //     The browser home key (Windows 2000 or later).
            BrowserHome = 172,
            //
            // Summary:
            //     The volume mute key (Windows 2000 or later).
            VolumeMute = 173,
            //
            // Summary:
            //     The volume down key (Windows 2000 or later).
            VolumeDown = 174,
            //
            // Summary:
            //     The volume up key (Windows 2000 or later).
            VolumeUp = 175,
            //
            // Summary:
            //     The media next track key (Windows 2000 or later).
            MediaNextTrack = 176,
            //
            // Summary:
            //     The media previous track key (Windows 2000 or later).
            MediaPreviousTrack = 177,
            //
            // Summary:
            //     The media Stop key (Windows 2000 or later).
            MediaStop = 178,
            //
            // Summary:
            //     The media play pause key (Windows 2000 or later).
            MediaPlayPause = 179,
            //
            // Summary:
            //     The launch mail key (Windows 2000 or later).
            LaunchMail = 180,
            //
            // Summary:
            //     The select media key (Windows 2000 or later).
            SelectMedia = 181,
            //
            // Summary:
            //     The start application one key (Windows 2000 or later).
            LaunchApplication1 = 182,
            //
            // Summary:
            //     The start application two key (Windows 2000 or later).
            LaunchApplication2 = 183,
            //
            // Summary:
            //     The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
            OemSemicolon = 186,
            //
            // Summary:
            //     The OEM 1 key.
            Oem1 = 186,
            //
            // Summary:
            //     The OEM plus key on any country/region keyboard (Windows 2000 or later).
            Oemplus = 187,
            //
            // Summary:
            //     The OEM comma key on any country/region keyboard (Windows 2000 or later).
            Oemcomma = 188,
            //
            // Summary:
            //     The OEM minus key on any country/region keyboard (Windows 2000 or later).
            OemMinus = 189,
            //
            // Summary:
            //     The OEM period key on any country/region keyboard (Windows 2000 or later).
            OemPeriod = 190,
            //
            // Summary:
            //     The OEM question mark key on a US standard keyboard (Windows 2000 or later).
            OemQuestion = 191,
            //
            // Summary:
            //     The OEM 2 key.
            Oem2 = 191,
            //
            // Summary:
            //     The OEM tilde key on a US standard keyboard (Windows 2000 or later).
            Oemtilde = 192,
            //
            // Summary:
            //     The OEM 3 key.
            Oem3 = 192,
            //
            // Summary:
            //     The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
            OemOpenBrackets = 219,
            //
            // Summary:
            //     The OEM 4 key.
            Oem4 = 219,
            //
            // Summary:
            //     The OEM pipe key on a US standard keyboard (Windows 2000 or later).
            OemPipe = 220,
            //
            // Summary:
            //     The OEM 5 key.
            Oem5 = 220,
            //
            // Summary:
            //     The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
            OemCloseBrackets = 221,
            //
            // Summary:
            //     The OEM 6 key.
            Oem6 = 221,
            //
            // Summary:
            //     The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
            OemQuotes = 222,
            //
            // Summary:
            //     The OEM 7 key.
            Oem7 = 222,
            //
            // Summary:
            //     The OEM 8 key.
            Oem8 = 223,
            //
            // Summary:
            //     The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000
            //     or later).
            OemBackslash = 226,
            //
            // Summary:
            //     The OEM 102 key.
            Oem102 = 226,
            //
            // Summary:
            //     The PROCESS KEY key.
            ProcessKey = 229,
            //
            // Summary:
            //     Used to pass Unicode characters as if they were keystrokes. The Packet key value
            //     is the low word of a 32-bit virtual-key value used for non-keyboard input methods.
            Packet = 231,
            //
            // Summary:
            //     The ATTN key.
            Attn = 246,
            //
            // Summary:
            //     The CRSEL key.
            Crsel = 247,
            //
            // Summary:
            //     The EXSEL key.
            Exsel = 248,
            //
            // Summary:
            //     The ERASE EOF key.
            EraseEof = 249,
            //
            // Summary:
            //     The PLAY key.
            Play = 250,
            //
            // Summary:
            //     The ZOOM key.
            Zoom = 251,
            //
            // Summary:
            //     A constant reserved for future use.
            NoName = 252,
            //
            // Summary:
            //     The PA1 key.
            Pa1 = 253,
            //
            // Summary:
            //     The CLEAR key.
            OemClear = 254,
            //
            // Summary:
            //     The bitmask to extract a key code from a key value.
            KeyCode = 65535,
            //
            // Summary:
            //     The SHIFT modifier key.
            Shift = 65536,
            //
            // Summary:
            //     The CTRL modifier key.
            Control = 131072,
            //
            // Summary:
            //     The ALT modifier key.
            Alt = 262144
        }
#endif
}

