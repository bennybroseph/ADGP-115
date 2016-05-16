using System;
using Library;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml;
using System.Xml.Linq;

namespace Units
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
        void Register(IControlable a_Controlable);
        void UnRegister(IControlable a_Controlable);
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

    public struct UserConfiguration
    {
        public List<Key<KeyCode>> skillKeyCodes;
        public List<Key<ButtonCode>> skillButtonCodes;

        public Axis<KeyCode> verticalKeyAxis;
        public Axis<KeyCode> horizontalKeyAxis;

        public Axis<ButtonCode> verticalButtonAxis;
        public Axis<ButtonCode> horizontalButtonAxis;

        public UserConfiguration(
            List<Key<KeyCode>> a_SkillKeyCodes,
            List<Key<ButtonCode>> a_SkillButtonCodes,
            Axis<KeyCode> a_VerticalKeyAxis,
            Axis<KeyCode> a_HorizontalKeyAxis,
            Axis<ButtonCode> a_VerticalButtonAxis,
            Axis<ButtonCode> a_HorizontalButtonAxis)
        {
            skillKeyCodes = a_SkillKeyCodes;
            skillButtonCodes = a_SkillButtonCodes;

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
                new XDocument().Save(Globals.FILE_NAME);
                SetValuesToDefault();
                WriteConfig();
            }
            else
                ReadConfig();
        }
        #region -- READ XML --
        private void ReadConfig()
        {
            m_UserConfigurations = new List<UserConfiguration>();

            using (XmlReader reader = XmlReader.Create(Globals.FILE_NAME))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "Player":
                                {
                                    List<Key<KeyCode>> keySkills = new List<Key<KeyCode>>();
                                    List<Axis<KeyCode>> keyAxii = new List<Axis<KeyCode>>();

                                    List<Key<ButtonCode>> buttonSkills = new List<Key<ButtonCode>>();
                                    List<Axis<ButtonCode>> buttonAxii = new List<Axis<ButtonCode>>();

                                    while (reader.Read())
                                    {
                                        if (reader.IsStartElement())
                                        {
                                            switch (reader.Name)
                                            {
                                                case "Keyboard":
                                                    {
                                                        while (reader.Read())
                                                        {
                                                            if (reader.IsStartElement())
                                                            {
                                                                if (reader.Name == "Axis")
                                                                    break;
                                                                keySkills.Add(ReadKey<KeyCode>(reader));
                                                            }
                                                        }

                                                        for (int i = 0; i < 2; i++)
                                                        {
                                                            Axis<KeyCode> axis = new Axis<KeyCode>();
                                                            while (reader.Read())
                                                            {
                                                                if (reader.IsStartElement())
                                                                {
                                                                    switch (reader.Name)
                                                                    {
                                                                        case "Positive":
                                                                            axis.positive = ReadKey<KeyCode>(reader);
                                                                            break;
                                                                        case "Negative":
                                                                            axis.negative = ReadKey<KeyCode>(reader);
                                                                            break;
                                                                    }
                                                                }
                                                                else if (reader.Name == "Axis")
                                                                    break;
                                                            }
                                                            keyAxii.Add(new Axis<KeyCode>(axis.positive, axis.negative));
                                                        }
                                                    }
                                                    break;
                                                case "XInput":
                                                    {
                                                        while (reader.Read())
                                                        {
                                                            if (reader.IsStartElement())
                                                            {
                                                                if (reader.Name == "Axis")
                                                                    break;
                                                                buttonSkills.Add(ReadKey<ButtonCode>(reader));
                                                            }
                                                        }

                                                        for (int i = 0; i < 2; i++)
                                                        {
                                                            Axis<ButtonCode> axis = new Axis<ButtonCode>();
                                                            while (reader.Read())
                                                            {
                                                                if (reader.IsStartElement())
                                                                {
                                                                    switch (reader.Name)
                                                                    {
                                                                        case "Positive":
                                                                            axis.positive = ReadKey<ButtonCode>(reader);
                                                                            break;
                                                                        case "Negative":
                                                                            axis.negative = ReadKey<ButtonCode>(reader);
                                                                            break;
                                                                    }
                                                                }
                                                                else if (reader.Name == "Axis")
                                                                    break;
                                                            }
                                                            buttonAxii.Add(new Axis<ButtonCode>(axis.positive, axis.negative));
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                        else if (reader.Name == "Player")
                                            break;
                                    }
                                    m_UserConfigurations.Add(
                                        new UserConfiguration(
                                            keySkills,
                                            buttonSkills,
                                            keyAxii[0],
                                            keyAxii[1],
                                            buttonAxii[0],
                                            buttonAxii[1]));
                                }
                                break;
                        }
                    }
                }
            }
        }

        private Key<TCode> ReadKey<TCode>(XmlReader a_Reader)
        {
            Key<TCode> key;

            //a_Reader.Read();

            a_Reader.MoveToNextAttribute();
            string name = a_Reader.Value;

            a_Reader.MoveToNextAttribute();
            string description = a_Reader.Value;

            a_Reader.MoveToNextAttribute();
            TCode keyCode = (TCode)Enum.Parse(typeof(TCode), a_Reader.Value);

            key = new Key<TCode>(name, description, keyCode);
            //Debug.Log(name + " " + description + " " + keyCode);
            return key;
        }
        #endregion

        #region -- WRITE TO XML --

        private void SetValuesToDefault()
        {
            m_UserConfigurations = new List<UserConfiguration>
            {
                new UserConfiguration(
                    new List<Key<KeyCode>>
                    {
                        new Key<KeyCode>("Skill 1", "First Skill", KeyCode.Q),
                        new Key<KeyCode>("Skill 2", "Second Skill", KeyCode.E),
                    },
                    new List<Key<ButtonCode>>
                    {
                        new Key<ButtonCode>("Skill 1", "First Skill", ButtonCode.X),
                        new Key<ButtonCode>("Skill 2", "First Skill", ButtonCode.A),
                    },
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
                    new List<Key<KeyCode>>
                    {
                        new Key<KeyCode>("Skill 1", "First SKill", KeyCode.Keypad1),
                        new Key<KeyCode>("Skill 2", "Second SKill", KeyCode.Keypad0),
                    },
                    new List<Key<ButtonCode>>
                    {
                        new Key<ButtonCode>("Skill 1", "First Skill", ButtonCode.X),
                        new Key<ButtonCode>("Skill 2", "First Skill", ButtonCode.A),
                    },
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

        public void WriteConfig()
        {
            using (XmlWriter writer = XmlWriter.Create(Globals.FILE_NAME))
            {
                writer.WriteStartDocument();
                {
                    writer.WriteStartElement("Config");
                    {
                        foreach (UserConfiguration keyConfiguration in m_UserConfigurations)
                        {
                            writer.WriteStartElement("Player");
                            {
                                writer.WriteStartElement("Keyboard");
                                {
                                    foreach (Key<KeyCode> key in keyConfiguration.skillKeyCodes)
                                    {
                                        writer.WriteStartElement("Skill");
                                        {
                                            WriteKey(writer, key);
                                        }
                                        writer.WriteEndElement();
                                    }
                                    WriteAxii(writer,
                                        new List<Axis<KeyCode>>
                                        {
                                            keyConfiguration.verticalKeyAxis,
                                            keyConfiguration.horizontalKeyAxis,
                                        });
                                }
                                writer.WriteEndElement();

                                writer.WriteStartElement("XInput");
                                {
                                    foreach (Key<ButtonCode> key in keyConfiguration.skillButtonCodes)
                                    {
                                        writer.WriteStartElement("Skill");
                                        {
                                            WriteKey(writer, key);
                                        }
                                        writer.WriteEndElement();
                                    }
                                    WriteAxii(writer,
                                        new List<Axis<ButtonCode>>
                                        {
                                            keyConfiguration.verticalButtonAxis,
                                            keyConfiguration.horizontalButtonAxis,
                                        });
                                }
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndDocument();
            }
        }
        private static void WriteKey<TCode>(XmlWriter a_Writer, Key<TCode> a_Key)
        {
            a_Writer.WriteAttributeString("Name", a_Key.name);
            a_Writer.WriteAttributeString("Description", a_Key.description);
            a_Writer.WriteAttributeString("KeyCode", a_Key.keyCode.ToString());
        }

        private static void WriteAxis<TCode>(XmlWriter a_Writer, Axis<TCode> a_Axis)
        {
            a_Writer.WriteStartElement("Positive");
            {
                WriteKey(a_Writer, a_Axis.positive);
            }
            a_Writer.WriteEndElement();
            a_Writer.WriteStartElement("Negative");
            {
                WriteKey(a_Writer, a_Axis.negative);
            }
            a_Writer.WriteEndElement();
        }

        private static void WriteAxii<TCode>(XmlWriter a_Writer, List<Axis<TCode>> a_Axii)
        {
            foreach (Axis<TCode> axis in a_Axii)
            {
                a_Writer.WriteStartElement("Axis");
                {
                    WriteAxis(a_Writer, axis);
                }
                a_Writer.WriteEndElement();
            }
        }
        #endregion
    }
}
