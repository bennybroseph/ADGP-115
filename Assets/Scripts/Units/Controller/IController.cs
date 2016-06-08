using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Interfaces;
using Library;
using UnityEngine;

namespace Units.Controller
{
    internal static partial class Globals
    {
#if UNITY_EDITOR
        internal const string FILE_NAME = "Assets/Config/KeyBindings.xml";
#else
        internal const string FILE_NAME = "KeyBindings.xml";
#endif
    }

    public interface IController
    {
        void Register(IControllable a_Controllable);
        void Register(Player a_Player);

        void UnRegister(IControllable a_Controllable);
        void UnRegister(Player a_Player);
    }

    #region -- CONTROLS STRUCTS --
    public enum ButtonCode
    {
        A,
        B,
        X,
        Y,
        DpadUp,
        DpadDown,
        DpadLeft,
        DpadRight,
        Start,
        Back,
        LBumper,
        RBumper,
        LStick,
        RStick,
    }

    public struct Key<TCode>
    {
        public string name;
        public string description;

        public TCode keyCode;

        public Key(string a_Name, string a_Description, TCode a_KeyCode)
        {
            name = a_Name;
            description = a_Description;

            keyCode = a_KeyCode;
        }
    }

    public struct Axis<TCode>
    {
        public Key<TCode> positive;
        public Key<TCode> negative;

        public Axis(Key<TCode> a_Positive, Key<TCode> a_Negative)
        {
            positive = a_Positive;
            negative = a_Negative;
        }
    }

    public class UserConfiguration
    {
        public Key<KeyCode> pauseKey;
        public Key<ButtonCode> pauseButton;

        public List<Key<KeyCode>> skillKeys;
        public List<Key<ButtonCode>> skillButtons;

        public Key<KeyCode> targetModeKey;
        public Key<KeyCode> switchTargetKey;

        public Key<ButtonCode> targetModeButton;

        public Axis<KeyCode> verticalKeyAxis;
        public Axis<KeyCode> horizontalKeyAxis;

        public Axis<ButtonCode> verticalButtonAxis;
        public Axis<ButtonCode> horizontalButtonAxis;

        public UserConfiguration() { }

        public UserConfiguration(
            Key<KeyCode> a_PauseKey,
            Key<ButtonCode> a_PauseButton,

            List<Key<KeyCode>> a_SkillKeys,
            List<Key<ButtonCode>> a_SkillButtons,

            Key<KeyCode> a_TargetModeKey,
            Key<KeyCode> a_SwitchTargetKey,

            Key<ButtonCode> a_TargetModeButton,

            Axis<KeyCode> a_VerticalKeyAxis,
            Axis<KeyCode> a_HorizontalKeyAxis,

            Axis<ButtonCode> a_VerticalButtonAxis,
            Axis<ButtonCode> a_HorizontalButtonAxis)
        {
            pauseKey = a_PauseKey;
            pauseButton = a_PauseButton;

            skillKeys = a_SkillKeys;
            skillButtons = a_SkillButtons;

            targetModeKey = a_TargetModeKey;
            switchTargetKey = a_SwitchTargetKey;

            targetModeButton = a_TargetModeButton;

            verticalKeyAxis = a_VerticalKeyAxis;
            horizontalKeyAxis = a_HorizontalKeyAxis;

            verticalButtonAxis = a_VerticalButtonAxis;
            horizontalButtonAxis = a_HorizontalButtonAxis;
        }
    }
    #endregion

    public class KeyConfiguration : Singleton<KeyConfiguration>
    {
        private List<UserConfiguration> m_UserConfigurations;

        public List<UserConfiguration> userConfigurations
        {
            get { return m_UserConfigurations; }
            private set { m_UserConfigurations = value; }
        }

        public KeyConfiguration()
        {
            if (!File.Exists(Globals.FILE_NAME))
            {
                SetValuesToDefault();
                WriteConfig();
            }
            else
                ReadConfig();
        }
        #region -- READ XML --
        private void ReadConfig()
        {
            Debug.Log("Reading");

            XmlSerializer serializer = new XmlSerializer(typeof(List<UserConfiguration>));

            FileStream fileStream = new FileStream(Globals.FILE_NAME, FileMode.Open);
            XmlReader reader = XmlReader.Create(fileStream);

            m_UserConfigurations = (List<UserConfiguration>)serializer.Deserialize(reader);

            fileStream.Close();

            Debug.Log("Read");
        }

        private void WriteConfig()
        {
            Debug.Log("Writting");

            XmlSerializer serializer = new XmlSerializer(typeof(List<UserConfiguration>));

            TextWriter writer = new StreamWriter(Globals.FILE_NAME);
            serializer.Serialize(writer, m_UserConfigurations);
            writer.Close();

            Debug.Log("Written");
        }

        //private void ReadConfig()
        //{
        //    m_UserConfigurations = new List<UserConfiguration>();

        //    using (XmlReader reader = XmlReader.Create(Globals.FILE_NAME))
        //    {
        //        while (reader.Read())
        //        {
        //            if (reader.IsStartElement())
        //            {
        //                switch (reader.Name)
        //                {
        //                    case "Player":
        //                        {
        //                            List<Key<KeyCode>> keySkills = new List<Key<KeyCode>>();
        //                            List<Axis<KeyCode>> keyAxii = new List<Axis<KeyCode>>();

        //                            List<Key<ButtonCode>> buttonSkills = new List<Key<ButtonCode>>();
        //                            List<Axis<ButtonCode>> buttonAxii = new List<Axis<ButtonCode>>();

        //                            while (reader.Read())
        //                            {
        //                                if (reader.IsStartElement())
        //                                {
        //                                    switch (reader.Name)
        //                                    {
        //                                        case "Keyboard":
        //                                            {
        //                                                while (reader.Read())
        //                                                {
        //                                                    if (reader.IsStartElement())
        //                                                    {
        //                                                        if (reader.Name == "Axis")
        //                                                            break;
        //                                                        keySkills.Add(ReadKey<KeyCode>(reader));
        //                                                    }
        //                                                }

        //                                                for (int i = 0; i < 2; i++)
        //                                                {
        //                                                    Axis<KeyCode> axis = new Axis<KeyCode>();
        //                                                    while (reader.Read())
        //                                                    {
        //                                                        if (reader.IsStartElement())
        //                                                        {
        //                                                            switch (reader.Name)
        //                                                            {
        //                                                                case "Positive":
        //                                                                    axis.positive = ReadKey<KeyCode>(reader);
        //                                                                    break;
        //                                                                case "Negative":
        //                                                                    axis.negative = ReadKey<KeyCode>(reader);
        //                                                                    break;
        //                                                            }
        //                                                        }
        //                                                        else if (reader.Name == "Axis")
        //                                                            break;
        //                                                    }
        //                                                    keyAxii.Add(new Axis<KeyCode>(axis.positive, axis.negative));
        //                                                }
        //                                            }
        //                                            break;
        //                                        case "XInput":
        //                                            {
        //                                                while (reader.Read())
        //                                                {
        //                                                    if (reader.IsStartElement())
        //                                                    {
        //                                                        if (reader.Name == "Axis")
        //                                                            break;
        //                                                        buttonSkills.Add(ReadKey<ButtonCode>(reader));
        //                                                    }
        //                                                }

        //                                                for (int i = 0; i < 2; i++)
        //                                                {
        //                                                    Axis<ButtonCode> axis = new Axis<ButtonCode>();
        //                                                    while (reader.Read())
        //                                                    {
        //                                                        if (reader.IsStartElement())
        //                                                        {
        //                                                            switch (reader.Name)
        //                                                            {
        //                                                                case "Positive":
        //                                                                    axis.positive = ReadKey<ButtonCode>(reader);
        //                                                                    break;
        //                                                                case "Negative":
        //                                                                    axis.negative = ReadKey<ButtonCode>(reader);
        //                                                                    break;
        //                                                            }
        //                                                        }
        //                                                        else if (reader.Name == "Axis")
        //                                                            break;
        //                                                    }
        //                                                    buttonAxii.Add(new Axis<ButtonCode>(axis.positive, axis.negative));
        //                                                }
        //                                            }
        //                                            break;
        //                                    }
        //                                }
        //                                else if (reader.Name == "Player")
        //                                    break;
        //                            }
        //                            m_UserConfigurations.Add(
        //                                new UserConfiguration(
        //                                    keySkills,
        //                                    buttonSkills,
        //                                    keyAxii[0],
        //                                    keyAxii[1],
        //                                    buttonAxii[0],
        //                                    buttonAxii[1]));
        //                        }
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //}

        //private Key<TCode> ReadKey<TCode>(XmlReader a_Reader)
        //{
        //    Key<TCode> key;

        //    //a_Reader.Read();

        //    a_Reader.MoveToNextAttribute();
        //    string name = a_Reader.Value;

        //    a_Reader.MoveToNextAttribute();
        //    string description = a_Reader.Value;

        //    a_Reader.MoveToNextAttribute();
        //    TCode keyCode = (TCode)Enum.Parse(typeof(TCode), a_Reader.Value);

        //    key = new Key<TCode>(name, description, keyCode);
        //    //Debug.Log(name + " " + description + " " + keyCode);
        //    return key;
        //}
        #endregion

        #region -- WRITE TO XML --

        private void SetValuesToDefault()
        {
            m_UserConfigurations = new List<UserConfiguration>
            {
                new UserConfiguration(
                    new Key<KeyCode>("Pause", "Pause Key", KeyCode.P),
                    new Key<ButtonCode>("Pause", "Pause Controller Button", ButtonCode.Start),

                    new List<Key<KeyCode>>
                    {
                        new Key<KeyCode>("Skill 1", "First Skill", KeyCode.Q),
                        new Key<KeyCode>("Skill 2", "Second Skill", KeyCode.E),
                        new Key<KeyCode>("Skill 3", "Third Skill", KeyCode.R),
                    },
                    new List<Key<ButtonCode>>
                    {
                        new Key<ButtonCode>("Skill 1", "First Skill", ButtonCode.X),
                        new Key<ButtonCode>("Skill 2", "First Skill", ButtonCode.A),
                        new Key<ButtonCode>("Skill 3", "Third Skill", ButtonCode.B),
                    },

                    new Key<KeyCode>("Target", "Target Mode Toggle", KeyCode.F),
                    new Key<KeyCode>("Target", "Target Mode Toggle", KeyCode.Tab),

                    new Key<ButtonCode>("Target", "Target Mode Toggle", ButtonCode.RStick),

                    new Axis<KeyCode>(
                        new Key<KeyCode>("Up", "Up Key", KeyCode.W),
                        new Key<KeyCode>("Down", "Down Key", KeyCode.S)),
                    new Axis<KeyCode>(
                        new Key<KeyCode>("Right", "Right Key", KeyCode.D),
                        new Key<KeyCode>("Left", "Left Key", KeyCode.A)),

                    new Axis<ButtonCode>(
                        new Key<ButtonCode>("Up", "Up Key", ButtonCode.DpadUp),
                        new Key<ButtonCode>("Down", "Down Key", ButtonCode.DpadDown)),
                    new Axis<ButtonCode>(
                        new Key<ButtonCode>("Right", "Right Key", ButtonCode.DpadRight),
                        new Key<ButtonCode>("Left", "Left Key", ButtonCode.DpadLeft))),
                new UserConfiguration(
                    new Key<KeyCode>("Pause", "Pause Key", KeyCode.None),
                    new Key<ButtonCode>("Pause", "Pause Controller Button", ButtonCode.Start),

                    new List<Key<KeyCode>>
                    {
                        new Key<KeyCode>("Skill 1", "First SKill", KeyCode.Keypad1),
                        new Key<KeyCode>("Skill 2", "Second SKill", KeyCode.Keypad2),
                        new Key<KeyCode>("Skill 3", "Third Skill", KeyCode.Keypad3),
                    },
                    new List<Key<ButtonCode>>
                    {
                        new Key<ButtonCode>("Skill 1", "First Skill", ButtonCode.X),
                        new Key<ButtonCode>("Skill 2", "First Skill", ButtonCode.A),
                        new Key<ButtonCode>("Skill 3", "Third Skill", ButtonCode.B),
                    },

                    new Key<KeyCode>("Target", "Target Mode Toggle", KeyCode.KeypadPeriod),
                    new Key<KeyCode>("Target", "Target Mode Toggle", KeyCode.RightShift),

                    new Key<ButtonCode>("Target", "Target Mode Toggle", ButtonCode.RStick),

                    new Axis<KeyCode>(
                        new Key<KeyCode>("Up", "Up Key", KeyCode.UpArrow),
                        new Key<KeyCode>("Down", "Down Key", KeyCode.DownArrow)),
                    new Axis<KeyCode>(
                        new Key<KeyCode>("Right", "Right Key", KeyCode.RightArrow),
                        new Key<KeyCode>("Left", "Left Key", KeyCode.LeftArrow)),

                    new Axis<ButtonCode>(
                        new Key<ButtonCode>("Up", "Up Key", ButtonCode.DpadUp),
                        new Key<ButtonCode>("Down", "Down Key", ButtonCode.DpadDown)),
                    new Axis<ButtonCode>(
                        new Key<ButtonCode>("Right", "Right Key", ButtonCode.DpadRight),
                        new Key<ButtonCode>("Left", "Left Key", ButtonCode.DpadLeft))),
            };
        }

        //public void WriteConfig()
        //{
        //    using (XmlWriter writer = XmlWriter.Create(Globals.FILE_NAME))
        //    {
        //        writer.WriteStartDocument();
        //        {
        //            writer.WriteStartElement("Config");
        //            {
        //                foreach (UserConfiguration keyConfiguration in m_UserConfigurations)
        //                {
        //                    writer.WriteStartElement("Player");
        //                    {
        //                        writer.WriteStartElement("Keyboard");
        //                        {
        //                            foreach (Key<KeyCode> key in keyConfiguration.skillKeyCodes)
        //                            {
        //                                writer.WriteStartElement("Skill");
        //                                {
        //                                    WriteKey(writer, key);
        //                                }
        //                                writer.WriteEndElement();
        //                            }
        //                            WriteAxii(writer,
        //                                new List<Axis<KeyCode>>
        //                                {
        //                                    keyConfiguration.verticalKeyAxis,
        //                                    keyConfiguration.horizontalKeyAxis,
        //                                });
        //                        }
        //                        writer.WriteEndElement();

        //                        writer.WriteStartElement("XInput");
        //                        {
        //                            foreach (Key<ButtonCode> key in keyConfiguration.skillButtonCodes)
        //                            {
        //                                writer.WriteStartElement("Skill");
        //                                {
        //                                    WriteKey(writer, key);
        //                                }
        //                                writer.WriteEndElement();
        //                            }
        //                            WriteAxii(writer,
        //                                new List<Axis<ButtonCode>>
        //                                {
        //                                    keyConfiguration.verticalButtonAxis,
        //                                    keyConfiguration.horizontalButtonAxis,
        //                                });
        //                        }
        //                        writer.WriteEndElement();
        //                    }
        //                    writer.WriteEndElement();
        //                }
        //            }
        //            writer.WriteEndElement();
        //        }
        //        writer.WriteEndDocument();
        //    }
        //}
        //private static void WriteKey<TCode>(XmlWriter a_Writer, Key<TCode> a_Key)
        //{
        //    a_Writer.WriteAttributeString("Name", a_Key.name);
        //    a_Writer.WriteAttributeString("Description", a_Key.description);
        //    a_Writer.WriteAttributeString("KeyCode", a_Key.keyCode.ToString());
        //}

        //private static void WriteAxis<TCode>(XmlWriter a_Writer, Axis<TCode> a_Axis)
        //{
        //    a_Writer.WriteStartElement("Positive");
        //    {
        //        WriteKey(a_Writer, a_Axis.positive);
        //    }
        //    a_Writer.WriteEndElement();
        //    a_Writer.WriteStartElement("Negative");
        //    {
        //        WriteKey(a_Writer, a_Axis.negative);
        //    }
        //    a_Writer.WriteEndElement();
        //}

        //private static void WriteAxii<TCode>(XmlWriter a_Writer, List<Axis<TCode>> a_Axii)
        //{
        //    foreach (Axis<TCode> axis in a_Axii)
        //    {
        //        a_Writer.WriteStartElement("Axis");
        //        {
        //            WriteAxis(a_Writer, axis);
        //        }
        //        a_Writer.WriteEndElement();
        //    }
        //}
        #endregion
    }
}
