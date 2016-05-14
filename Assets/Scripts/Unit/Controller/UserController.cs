using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Library;
using UnityEngine;
using XInputDotNetPure;

using Event = Define.Event;

namespace Unit.Controller
{
    internal static class Globals
    {
#if UNITY_EDITOR
        internal const string FILE_NAME = "Assets/Resources/KeyBindings.xml";
#else
        internal const string FILE_NAME = "KeyBindings.xml";
#endif
    }

    #region -- CONTROLS STRUCTS --
    internal struct Key
    {
        public string name;
        public string description;

        public KeyCode keyCode;

        public Key(string a_Name, string a_Description, KeyCode a_KeyCode)
        {
            name = a_Name;
            description = a_Description;

            keyCode = a_KeyCode;
        }
    }

    internal struct Axis
    {
        public Key positive;
        public Key negative;

        public Axis(Key a_Positive, Key a_Negative)
        {
            positive = a_Positive;
            negative = a_Negative;
        }
    }

    internal struct KeyConfiguration
    {
        public List<Key> skillKeyCodes;

        public Axis vertical;
        public Axis horizontal;

        public KeyConfiguration(List<Key> a_SkillKeyCodes, Axis a_Vertical, Axis a_Horizontal)
        {
            skillKeyCodes = a_SkillKeyCodes;

            vertical = a_Vertical;
            horizontal = a_Horizontal;
        }
    }
    #endregion

    public sealed class UserController : MonoSingleton<UserController>, IController
    {
        private List<KeyConfiguration> m_UserControls;
        [SerializeField]
        private List<IControlable> m_Controlables;

        public UserController()
        {
            if (!File.Exists(Globals.FILE_NAME))
            {
                new XDocument().Save(Globals.FILE_NAME);
                CreateDefaultXML();
            }
            else
                ReadXML();

            m_Controlables = new List<IControlable>();
        }

        private void FixedUpdate()
        {
            foreach (IControlable controlable in m_Controlables)
            {
                controlable.transform.position += (controlable.velocity + controlable.totalVelocity) * Time.deltaTime;

                if (controlable.velocity != Vector3.zero &&
                    (controlable.isMoving == Moving.nowhere ||
#if !UNITY_WEBGL
                        (GameManager.self.state.ThumbSticks.Left.X == 0.0f &&
                         GameManager.self.state.ThumbSticks.Left.Y == 0.0f)))
#else
                    (Input.GetAxisRaw("Horizontal") == 0.0f &&
                    Input.GetAxisRaw("Vertical") == 0.0f)))
#endif
                {
                    Movable.Brake(controlable);
                }
            }
        }

        private void Update()
        {
            int i = 0;
            foreach (IControlable controlable in m_Controlables)
            {
                if (i >= m_UserControls.Count)
                    continue;

                if (controlable.canMoveWithInput)
                {
                    Moving dPad = Moving.nowhere;
#if !UNITY_WEBGL
                    dPad.forward = GameManager.self.state.DPad.Up == ButtonState.Pressed;
                    dPad.back = GameManager.self.state.DPad.Down == ButtonState.Pressed;
                    dPad.left = GameManager.self.state.DPad.Left == ButtonState.Pressed;
                    dPad.right = GameManager.self.state.DPad.Right == ButtonState.Pressed;
#else
                    dPad.forward = Input.GetAxisRaw("POV Vertical") > 0.0f;
                    dPad.back = Input.GetAxisRaw("POV Vertical") < 0.0f;
                    dPad.left = Input.GetAxisRaw("POV Horizontal") < 0.0f;
                    dPad.right = Input.GetAxisRaw("POV Horizontal") > 0.0f;
#endif
                    controlable.isMoving = new Moving
                    {
                        forward = Input.GetKey(m_UserControls[i].vertical.positive.keyCode) | dPad.forward,
                        back = Input.GetKey(m_UserControls[i].vertical.negative.keyCode) | dPad.back,
                        left = Input.GetKey(m_UserControls[i].horizontal.negative.keyCode) | dPad.left,
                        right = Input.GetKey(m_UserControls[i].horizontal.positive.keyCode) | dPad.right
                    };

                    if (controlable.isMoving.forward)
                        controlable.velocity = new Vector3(0, controlable.velocity.y, controlable.speed);
                    if (controlable.isMoving.back)
                        controlable.velocity = new Vector3(0, controlable.velocity.y, -controlable.speed);
                    if (controlable.isMoving.left)
                        controlable.velocity = new Vector3(-controlable.speed, controlable.velocity.y, 0);
                    if (controlable.isMoving.right)
                        controlable.velocity = new Vector3(controlable.speed, controlable.velocity.y, 0);

                    if (controlable.isMoving.forward && controlable.isMoving.left)
                        controlable.velocity = new Vector3(-Mathf.Sqrt(controlable.speed * 2), controlable.velocity.y, Mathf.Sqrt(controlable.speed * 2));
                    if (controlable.isMoving.forward && controlable.isMoving.right)
                        controlable.velocity = new Vector3(Mathf.Sqrt(controlable.speed * 2), controlable.velocity.y, Mathf.Sqrt(controlable.speed * 2));
                    if (controlable.isMoving.back && controlable.isMoving.left)
                        controlable.velocity = new Vector3(-Mathf.Sqrt(controlable.speed * 2), controlable.velocity.y, -Mathf.Sqrt(controlable.speed * 2));
                    if (controlable.isMoving.back && controlable.isMoving.right)
                        controlable.velocity = new Vector3(Mathf.Sqrt(controlable.speed * 2), controlable.velocity.y, -Mathf.Sqrt(controlable.speed * 2));

                    Vector2 leftStick;
#if !UNITY_WEBGL
                    leftStick.x = GameManager.self.state.ThumbSticks.Left.X;
                    leftStick.y = GameManager.self.state.ThumbSticks.Left.Y;
#else
                    leftStick.x = Input.GetAxisRaw("Horizontal");
                    leftStick.y = Input.GetAxisRaw("Vertical");
#endif
                    if (leftStick.x != 0.0f ||
                        leftStick.y != 0.0f)
                    {
                        controlable.velocity = new Vector3(
                            leftStick.x * controlable.speed,
                            controlable.velocity.y,
                            leftStick.y * controlable.speed);
                    }
                }

                bool[] isPressed;
#if !UNITY_WEBGL
                isPressed = new[]
                {
                    GameManager.self.state.Triggers.Right > 0.0f,
                    GameManager.self.state.Triggers.Left > 0.0f
                };
#else
                isPressed = new[]
                {
                Input.GetAxisRaw("Skill 1") != 0.0f,
                Input.GetAxisRaw("Skill 2") != 0.0f,
                Input.GetAxisRaw("Skill 3") != 0.0f,
                Input.GetAxisRaw("Skill 4") != 0.0f
            };
#endif

                if (Input.GetKeyDown(KeyCode.Q) || isPressed[0])
                    Publisher.self.Broadcast(Event.UseSkill, 1);
                if (Input.GetKeyDown(KeyCode.E) || isPressed[1])
                    Publisher.self.Broadcast(Event.UseSkill, 2);

                i++;
            }
        }

        public void Register(IControlable a_Controlable)
        {
            m_Controlables.Add(a_Controlable);

            a_Controlable.movementFSM.AddTransition(MovementState.Init, MovementState.Idle);
            a_Controlable.movementFSM.AddTransition(MovementState.Idle, MovementState.Walking);
            a_Controlable.movementFSM.AddTransition(MovementState.Walking, MovementState.Running);

            a_Controlable.movementFSM.AddTransition(MovementState.Running, MovementState.Walking);
            a_Controlable.movementFSM.AddTransition(MovementState.Walking, MovementState.Idle);
        }

        #region -- READ XML --
        private void ReadXML()
        {
            m_UserControls = new List<KeyConfiguration>();

            using (XmlReader reader = XmlReader.Create(Globals.FILE_NAME))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "Player":
                                m_UserControls.Add(new KeyConfiguration(new List<Key>(), new Axis(), new Axis()));
                                break;
                            case "Skill":
                                m_UserControls[m_UserControls.Count - 1].skillKeyCodes.Add(ReadKey(reader));
                                break;
                            case "Axis":
                                {
                                    reader.Read();

                                    Key positive = ReadKey(reader);
                                    reader.Read();
                                    reader.Read();
                                    Key negative = ReadKey(reader);

                                    m_UserControls[m_UserControls.Count - 1] = new KeyConfiguration(
                                        m_UserControls[m_UserControls.Count - 1].skillKeyCodes,
                                        new Axis(positive, negative),
                                        m_UserControls[m_UserControls.Count - 1].horizontal);
                                    reader.Read(); reader.Read(); reader.Read(); reader.Read();
                                    Debug.Log(reader.Name);

                                    positive = ReadKey(reader);
                                    reader.Read();
                                    reader.Read();
                                    negative = ReadKey(reader);

                                    m_UserControls[m_UserControls.Count - 1] = new KeyConfiguration(
                                        m_UserControls[m_UserControls.Count - 1].skillKeyCodes,
                                        m_UserControls[m_UserControls.Count - 1].vertical,
                                        new Axis(positive, negative));
                                }
                                break;
                        }
                    }
                }
            }
        }

        private Key ReadKey(XmlReader a_Reader)
        {
            Key key;

            a_Reader.Read();

            a_Reader.MoveToNextAttribute();
            string name = a_Reader.Value;

            a_Reader.MoveToNextAttribute();
            string description = a_Reader.Value;

            a_Reader.MoveToNextAttribute();
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), a_Reader.Value);

            key = new Key(name, description, keyCode);
            Debug.Log(name + " " + description + " " + keyCode);
            return key;
        }
        #endregion

        #region -- WRITE TO XML --
        public void CreateDefaultXML()
        {
            m_UserControls = new List<KeyConfiguration>
            {
                new KeyConfiguration(
                    new List<Key>
                    {
                        new Key("Skill 1", "First SKill", KeyCode.Q),
                        new Key("Skill 2", "Second SKill", KeyCode.E),
                    },
                    new Axis(
                        new Key("Up", "Up Key", KeyCode.W),
                        new Key("Down", "Down Key", KeyCode.S)),
                    new Axis(
                        new Key("Right", "Right Key", KeyCode.D),
                        new Key("Left", "Left Key", KeyCode.A))),
                new KeyConfiguration(
                    new List<Key>
                    {
                        new Key("Skill 1", "First SKill", KeyCode.Keypad1),
                        new Key("Skill 2", "Second SKill", KeyCode.Keypad0),
                    },
                    new Axis(
                        new Key("Up", "Up Key", KeyCode.UpArrow),
                        new Key("Down", "Down Key", KeyCode.DownArrow)),
                    new Axis(
                        new Key("Right", "Right Key", KeyCode.RightArrow),
                        new Key("Left", "Left Key", KeyCode.LeftArrow))),
            };

            using (XmlWriter writer = XmlWriter.Create(Globals.FILE_NAME))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Config");
                foreach (KeyConfiguration keyConfiguration in m_UserControls)
                {
                    writer.WriteStartElement("Player");
                    foreach (Key key in keyConfiguration.skillKeyCodes)
                    {
                        writer.WriteStartElement("Skill");
                        WriteKey(writer, key);
                        writer.WriteEndElement();
                    }
                    writer.WriteStartElement("Axis");
                    WriteAxis(writer, keyConfiguration.horizontal);
                    writer.WriteEndElement();

                    writer.WriteStartElement("Axis");
                    WriteAxis(writer, keyConfiguration.vertical);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
        private void WriteKey(XmlWriter a_Writer, Key a_Key)
        {
            a_Writer.WriteStartElement("Key");
            a_Writer.WriteAttributeString("Name", a_Key.name);
            a_Writer.WriteAttributeString("Description", a_Key.description);
            a_Writer.WriteAttributeString("KeyCode", a_Key.keyCode.ToString());
            a_Writer.WriteEndElement();
        }

        private void WriteAxis(XmlWriter a_Writer, Axis a_Axis)
        {
            a_Writer.WriteStartElement("Positive");
            WriteKey(a_Writer, a_Axis.positive);
            a_Writer.WriteEndElement();
            a_Writer.WriteStartElement("Negative");
            WriteKey(a_Writer, a_Axis.negative);
            a_Writer.WriteEndElement();
        }
        #endregion
    }
}