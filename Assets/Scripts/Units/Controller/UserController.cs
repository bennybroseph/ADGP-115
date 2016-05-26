using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interfaces;
using UnityEngine;

using Library;
using UnityEngine.EventSystems;
using Event = Define.Event;

namespace Units.Controller
{
    public sealed class UserController : MonoSingleton<UserController>, IController
    {
#if !UNITY_WEBGL
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
#endif
        [SerializeField]
        private List<IControllable> m_Controllables;
        [SerializeField]
        private EventSystem m_EventSystem;

        private Vector2 m_MouseAnchor;

        private Vector2 m_PrevMousePosition;
        private Vector2 m_DeltaMousePosition;

        public List<IControllable> controllables
        {
            get { return m_Controllables; }
        }

        #region -- UNITY FUNCTIONS --
        protected override void Awake()
        {
            base.Awake();

            m_Controllables = new List<IControllable>();
        }

        private void Start()
        {
            m_EventSystem = FindObjectOfType(typeof(EventSystem)) as EventSystem;
        }

        private void FixedUpdate()
        {
            int i = 0;
            foreach (IControllable controlable in m_Controllables)
            {
                controlable.gameObject.GetComponent<Rigidbody>().velocity = (controlable.velocity + controlable.totalVelocity);

                if (controlable.velocity != Vector3.zero &&
                    (controlable.isMoving == Moving.nowhere) &&
#if !UNITY_WEBGL
                    (GameManager.self.GetStickValue(i, GameManager.Stick.Left).X == 0.0f &&
                     GameManager.self.GetStickValue(i, GameManager.Stick.Left).Y == 0.0f))
#else
                    (Input.GetAxisRaw("Horizontal") == 0.0f &&
                     Input.GetAxisRaw("Vertical") == 0.0f))
#endif
                {
                    controlable.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                i++;
            }
        }

        private void Update()
        {
#if !UNITY_WEBGL
            POINT currentMousePosition;
            GetCursorPos(out currentMousePosition);

            m_DeltaMousePosition =
                new Vector2(currentMousePosition.X, currentMousePosition.Y) - m_PrevMousePosition;

            if (Input.GetMouseButtonDown(1))
                m_MouseAnchor = new Vector2(currentMousePosition.X, currentMousePosition.Y);

            if (Input.GetMouseButton(1))
            {
                Cursor.visible = false;

                SetCursorPos((int)m_MouseAnchor.x, (int)m_MouseAnchor.y);
                GetCursorPos(out currentMousePosition);

                Vector3 newAngle = ThirdPersonCamera.self.transform.eulerAngles;
                newAngle +=
                    new Vector3(
                        m_DeltaMousePosition.y * Time.deltaTime * 8f,
                        m_DeltaMousePosition.x * Time.deltaTime * 12f,
                        0);

                newAngle =
                        new Vector3(
                            Mathf.Clamp(newAngle.x, 10, 90),
                            newAngle.y,
                            newAngle.z);

                ThirdPersonCamera.self.transform.eulerAngles = newAngle;
            }
            else
                Cursor.visible = true;

            m_PrevMousePosition = new Vector2(currentMousePosition.X, currentMousePosition.Y);
#else
            if (m_EventSystem.currentSelectedGameObject == null)
            {
                Vector2 currentMousePosition = Input.mousePosition;
                m_DeltaMousePosition =
                    new Vector2(
                        Input.GetAxis("Mouse X") * Time.deltaTime * 1.2f, 
                        -Input.GetAxis("Mouse Y") * Time.deltaTime * 0.8f);

                if (Input.GetMouseButtonDown(1))
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    m_MouseAnchor = currentMousePosition;
                }
                if (Input.GetMouseButton(1))
                {
                    Vector3 newAngle = ThirdPersonCamera.self.transform.eulerAngles;
                    newAngle +=
                        new Vector3(
                            m_DeltaMousePosition.y * Time.deltaTime,
                            m_DeltaMousePosition.x * Time.deltaTime,
                            0);

                    newAngle =
                            new Vector3(
                                Mathf.Clamp(newAngle.x, 10, 90),
                                newAngle.y,
                                newAngle.z);

                    ThirdPersonCamera.self.transform.eulerAngles = newAngle;
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }

                if (currentMousePosition != m_MouseAnchor)
                    m_PrevMousePosition = currentMousePosition;
            }
#endif
            int i = 0;
            foreach (IControllable controlable in m_Controllables)
            {
                if (i >= KeyConfiguration.self.userConfigurations.Count)
                    continue;

                if (controlable.canMoveWithInput)
                {
                    controlable.velocity = Vector3.zero;

                    Moving dPad = Moving.nowhere;
#if !UNITY_WEBGL
                    dPad.forward = GameManager.self.GetButtonState(
                        i,
                        KeyConfiguration.self.userConfigurations[i].verticalButtonAxis.positive.keyCode);
                    dPad.back = GameManager.self.GetButtonState(
                        i,
                        KeyConfiguration.self.userConfigurations[i].verticalButtonAxis.negative.keyCode);
                    dPad.left = GameManager.self.GetButtonState(
                        i,
                        KeyConfiguration.self.userConfigurations[i].horizontalButtonAxis.negative.keyCode);
                    dPad.right = GameManager.self.GetButtonState(
                        i,
                        KeyConfiguration.self.userConfigurations[i].horizontalButtonAxis.positive.keyCode);
#else
                    dPad.forward = Input.GetAxisRaw("POV Vertical") > 0.0f;
                    dPad.back = Input.GetAxisRaw("POV Vertical") < 0.0f;
                    dPad.left = Input.GetAxisRaw("POV Horizontal") < 0.0f;
                    dPad.right = Input.GetAxisRaw("POV Horizontal") > 0.0f;
#endif
                    controlable.isMoving = new Moving
                    {
                        forward = Input.GetKey(KeyConfiguration.self.userConfigurations[i].verticalKeyAxis.positive.keyCode) | dPad.forward,
                        back = Input.GetKey(KeyConfiguration.self.userConfigurations[i].verticalKeyAxis.negative.keyCode) | dPad.back,
                        left = Input.GetKey(KeyConfiguration.self.userConfigurations[i].horizontalKeyAxis.negative.keyCode) | dPad.left,
                        right = Input.GetKey(KeyConfiguration.self.userConfigurations[i].horizontalKeyAxis.positive.keyCode) | dPad.right
                    };

                    if (controlable.isMoving.forward)
                        controlable.velocity += Vector3.forward;
                    if (controlable.isMoving.back)
                        controlable.velocity += Vector3.back;
                    if (controlable.isMoving.left)
                        controlable.velocity += Vector3.left;
                    if (controlable.isMoving.right)
                        controlable.velocity += Vector3.right;

                    if (controlable.velocity != Vector3.zero)
                    {
                        float angle = ThirdPersonCamera.self.transform.eulerAngles.y * (Mathf.PI / 180);
                        angle += Mathf.Atan(controlable.velocity.x / controlable.velocity.z);

                        if ((controlable.velocity.x < 0.0f && controlable.velocity.z < 0.0f) ||
                            (controlable.velocity.x > 0.0f && controlable.velocity.z < 0.0f) ||
                            (controlable.velocity.x == 0.0f && controlable.velocity.z < 0.0f))
                            angle += Mathf.PI;

                        controlable.velocity = new Vector3(
                            controlable.speed * Mathf.Sin(angle),
                            0,
                            controlable.speed * Mathf.Cos(angle));
                    }
                    if (Input.GetMouseButton(1))
                    {
                        controlable.transform.eulerAngles = new Vector3(
                            controlable.transform.eulerAngles.x,
                            ThirdPersonCamera.self.transform.eulerAngles.y,
                            controlable.transform.eulerAngles.z);
                    }

                    Vector2 leftStick;
                    Vector2 rightStick;
#if !UNITY_WEBGL
                    leftStick.x = GameManager.self.GetStickValue(i, GameManager.Stick.Left).X;
                    leftStick.y = GameManager.self.GetStickValue(i, GameManager.Stick.Left).Y;

                    rightStick.x = GameManager.self.GetStickValue(i, GameManager.Stick.Right).X;
                    rightStick.y = GameManager.self.GetStickValue(i, GameManager.Stick.Right).Y;

                    
#else
                    leftStick.x = Input.GetAxisRaw("Horizontal");
                    leftStick.y = Input.GetAxisRaw("Vertical");

                    rightStick.x = Input.GetAxisRaw("Right Stick X");
                    rightStick.y = Input.GetAxisRaw("Right Stick Y");
#endif
                    if (rightStick.x != 0.0f ||
                        rightStick.y != 0.0f)
                    {
                        float rotationY = -(Mathf.Atan(rightStick.y / rightStick.x) * (180 / Mathf.PI));

                        if ((rightStick.x < 0.0f && rightStick.y > 0.0f) ||
                            (rightStick.x < 0.0f && rightStick.y < 0.0f) ||
                            (rightStick.x < 0.0f && rightStick.y == 0.0f))
                            rotationY += 180;

                        ThirdPersonCamera.self.transform.eulerAngles += new Vector3(
                            rightStick.y * 100 * Time.deltaTime,
                            rightStick.x * 100 * Time.deltaTime, 0);
                    }

                    if (leftStick.x != 0.0f ||
                        leftStick.y != 0.0f)
                    {
                        controlable.velocity = new Vector3(
                            leftStick.x * controlable.speed,
                            controlable.velocity.y,
                            leftStick.y * controlable.speed);

                        float angle = ThirdPersonCamera.self.transform.eulerAngles.y * (Mathf.PI / 180);
                        angle += Mathf.Atan(controlable.velocity.x / controlable.velocity.z);

                        if ((controlable.velocity.x < 0.0f && controlable.velocity.z < 0.0f) ||
                            (controlable.velocity.x > 0.0f && controlable.velocity.z < 0.0f) ||
                            (controlable.velocity.x == 0.0f && controlable.velocity.z < 0.0f))
                            angle += Mathf.PI;

                        controlable.velocity = new Vector3(
                            controlable.speed * Mathf.Sin(angle),
                            0,
                            controlable.speed * Mathf.Cos(angle));

                        controlable.transform.eulerAngles = new Vector3(
                            controlable.transform.eulerAngles.x,
                            angle * (180f / Mathf.PI),
                            controlable.transform.eulerAngles.z);
                    }
                }

#if !UNITY_WEBGL
                bool[] isPressed =
                {
                    GameManager.self.GetButtonState(
                        i,
                        KeyConfiguration.self.userConfigurations[i].skillButtons[0].keyCode),
                    GameManager.self.GetButtonState(
                        i,
                        KeyConfiguration.self.userConfigurations[i].skillButtons[1].keyCode),
                };
#else
                bool[] isPressed =
                {
                    Input.GetAxisRaw("Skill 1") != 0.0f,
                    Input.GetAxisRaw("Skill 2") != 0.0f,
                    Input.GetAxisRaw("Skill 3") != 0.0f,
                    Input.GetAxisRaw("Skill 4") != 0.0f
                };
#endif
                IUsesSkills skillUser = controlable as IUsesSkills;

                if (skillUser != null)
                {
                    if (Input.GetKeyDown(KeyConfiguration.self.userConfigurations[i].skillKeys[0].keyCode) || isPressed[0])
                        Publisher.self.Broadcast(Event.UseSkill, skillUser, 0);
                    if (Input.GetKeyDown(KeyConfiguration.self.userConfigurations[i].skillKeys[1].keyCode) || isPressed[1])
                        Publisher.self.Broadcast(Event.UseSkill, skillUser, 1);
                }

                i++;
            }
        }
        #endregion

        public void Register(IControllable a_Controllable)
        {
            m_Controllables.Add(a_Controllable);

            a_Controllable.movementFSM.AddTransition(MovementState.Init, MovementState.Idle);
            a_Controllable.movementFSM.AddTransition(MovementState.Idle, MovementState.Walking);
            a_Controllable.movementFSM.AddTransition(MovementState.Walking, MovementState.Running);

            a_Controllable.movementFSM.AddTransition(MovementState.Running, MovementState.Walking);
            a_Controllable.movementFSM.AddTransition(MovementState.Walking, MovementState.Idle);
        }
        public void UnRegister(IControllable a_Controllable)
        {
            m_Controllables.Remove(a_Controllable);
            if (m_Controllables.Count == 0)
                Publisher.self.Broadcast(Event.GameOver);
        }
    }
}