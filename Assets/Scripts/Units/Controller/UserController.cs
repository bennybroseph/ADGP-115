using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Interfaces;
using UnityEngine;

using Library;
using UnityEngine.EventSystems;
using Event = Define.Event;

namespace Units.Controller
{
    public sealed class UserController : MonoBehaviour, IController
    {
        [SerializeField]
        private Player m_Player;
        [SerializeField]
        private Unit m_Controllable;
        [SerializeField]
        private int m_PlayerIndex;

        public IControllable controllable
        {
            get { return m_Controllable; }
        }

        #region -- UNITY FUNCTIONS --
        private void FixedUpdate()
        {
            m_Controllable.gameObject.GetComponent<Rigidbody>().velocity = (m_Controllable.velocity + m_Controllable.totalVelocity);

            if (m_Controllable.velocity != Vector3.zero &&
                (m_Controllable.isMoving == Moving.nowhere) &&
#if !UNITY_WEBGL
                (GameManager.self.GetStickValue(m_PlayerIndex, GameManager.Stick.Left).X == 0.0f &&
                 GameManager.self.GetStickValue(m_PlayerIndex, GameManager.Stick.Left).Y == 0.0f))
#else
                (Input.GetAxisRaw("Horizontal") == 0.0f &&
                 Input.GetAxisRaw("Vertical") == 0.0f))
#endif
            {
                m_Controllable.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyConfiguration.self.userConfigurations[m_PlayerIndex].targetModeKey.keyCode) ||
#if !UNITY_WEBGL
                GameManager.self.GetButtonState(m_PlayerIndex, KeyConfiguration.self.userConfigurations[m_PlayerIndex].targetModeButton.keyCode))
#else
                false)
#endif
            {
                Publisher.self.Broadcast(Event.TargetTogglePressed);
            }

            if (Input.GetKeyDown(KeyConfiguration.self.userConfigurations[m_PlayerIndex].switchTargetKey.keyCode) &&
                m_Player.playerCamera.isTargeting)
            {
                Publisher.self.Broadcast(Event.TargetChangePressed);
            }

            if (m_Controllable.canMoveWithInput)
            {
                m_Controllable.velocity = Vector3.zero;

                Moving dPad = Moving.nowhere;
#if !UNITY_WEBGL
                dPad.forward = GameManager.self.GetButtonState(
                    m_PlayerIndex,
                    KeyConfiguration.self.userConfigurations[m_PlayerIndex].verticalButtonAxis.positive.keyCode);
                dPad.back = GameManager.self.GetButtonState(
                    m_PlayerIndex,
                    KeyConfiguration.self.userConfigurations[m_PlayerIndex].verticalButtonAxis.negative.keyCode);
                dPad.left = GameManager.self.GetButtonState(
                    m_PlayerIndex,
                    KeyConfiguration.self.userConfigurations[m_PlayerIndex].horizontalButtonAxis.negative.keyCode);
                dPad.right = GameManager.self.GetButtonState(
                    m_PlayerIndex,
                    KeyConfiguration.self.userConfigurations[m_PlayerIndex].horizontalButtonAxis.positive.keyCode);
#else
                dPad.forward = Input.GetAxisRaw("POV Vertical") > 0.0f;
                dPad.back = Input.GetAxisRaw("POV Vertical") < 0.0f;
                dPad.left = Input.GetAxisRaw("POV Horizontal") < 0.0f;
                dPad.right = Input.GetAxisRaw("POV Horizontal") > 0.0f;
#endif
                m_Controllable.isMoving = new Moving
                {
                    forward = Input.GetKey(KeyConfiguration.self.userConfigurations[m_PlayerIndex].verticalKeyAxis.positive.keyCode) | dPad.forward,
                    back = Input.GetKey(KeyConfiguration.self.userConfigurations[m_PlayerIndex].verticalKeyAxis.negative.keyCode) | dPad.back,
                    left = Input.GetKey(KeyConfiguration.self.userConfigurations[m_PlayerIndex].horizontalKeyAxis.negative.keyCode) | dPad.left,
                    right = Input.GetKey(KeyConfiguration.self.userConfigurations[m_PlayerIndex].horizontalKeyAxis.positive.keyCode) | dPad.right
                };

                if (m_Controllable.isMoving.forward)
                    m_Controllable.velocity += Vector3.forward;
                if (m_Controllable.isMoving.back)
                    m_Controllable.velocity += Vector3.back;
                if (m_Controllable.isMoving.left)
                    m_Controllable.velocity += Vector3.left;
                if (m_Controllable.isMoving.right)
                    m_Controllable.velocity += Vector3.right;

                if (m_Player.playerCamera.isTargeting)
                    m_Controllable.transform.eulerAngles = new Vector3(
                        m_Controllable.transform.eulerAngles.x,
                        m_Player.playerCamera.transform.eulerAngles.y + 25f,
                        m_Controllable.transform.eulerAngles.z);

                else if (Input.GetMouseButton(1))
                    m_Controllable.transform.eulerAngles = new Vector3(
                        m_Controllable.transform.eulerAngles.x,
                        m_Player.playerCamera.transform.eulerAngles.y,
                        m_Controllable.transform.eulerAngles.z);

                if (m_Controllable.velocity != Vector3.zero)
                {
                    float angle = 0f;

                    if (m_Player.playerCamera.isTargeting)
                        angle = (m_Player.playerCamera.transform.eulerAngles.y + 25f) * (Mathf.PI / 180);
                    else
                        angle = m_Player.playerCamera.transform.eulerAngles.y * (Mathf.PI / 180);
                    angle += Mathf.Atan(m_Controllable.velocity.x / m_Controllable.velocity.z);

                    if ((m_Controllable.velocity.x < 0.0f && m_Controllable.velocity.z < 0.0f) ||
                        (m_Controllable.velocity.x > 0.0f && m_Controllable.velocity.z < 0.0f) ||
                        (m_Controllable.velocity.x == 0.0f && m_Controllable.velocity.z < 0.0f))
                        angle += Mathf.PI;

                    m_Controllable.velocity = new Vector3(
                        m_Controllable.speed * Mathf.Sin(angle),
                        0,
                        m_Controllable.speed * Mathf.Cos(angle));

                    if (!Input.GetMouseButton(1) && !m_Player.playerCamera.isTargeting)
                        m_Controllable.transform.eulerAngles = new Vector3(
                            m_Controllable.transform.eulerAngles.x,
                            angle * (180 / Mathf.PI),
                            m_Controllable.transform.eulerAngles.z);
                }

                Vector2 leftStick;
                Vector2 rightStick;
#if !UNITY_WEBGL
                leftStick.x = GameManager.self.GetStickValue(m_PlayerIndex, GameManager.Stick.Left).X;
                leftStick.y = GameManager.self.GetStickValue(m_PlayerIndex, GameManager.Stick.Left).Y;

                rightStick.x = GameManager.self.GetStickValue(m_PlayerIndex, GameManager.Stick.Right).X;
                rightStick.y = GameManager.self.GetStickValue(m_PlayerIndex, GameManager.Stick.Right).Y;
#else
                leftStick.x = Input.GetAxisRaw("Horizontal");
                leftStick.y = Input.GetAxisRaw("Vertical");

                rightStick.x = Input.GetAxisRaw("Right Stick X");
                rightStick.y = Input.GetAxisRaw("Right Stick Y");
#endif
                if (rightStick.x != 0.0f ||
                    rightStick.y != 0.0f)
                {
                    if (m_Player.playerCamera.isTargeting)
                    {
                        if (Mathf.Abs(rightStick.x) > Mathf.Abs(rightStick.y))
                            ;
                    }
                    else
                    {
                        Vector3 newAngle = m_Player.playerCamera.transform.eulerAngles;
                        newAngle += new Vector3(
                            rightStick.y * 100 * Time.deltaTime,
                            rightStick.x * 100 * Time.deltaTime,
                            0);

                        newAngle = new Vector3(
                            Mathf.Clamp(
                                newAngle.x,
                                10,
                                90),
                            newAngle.y,
                            newAngle.z);

                        m_Player.playerCamera.transform.eulerAngles = newAngle;
                    }
                }

                if (leftStick.x != 0.0f ||
                    leftStick.y != 0.0f)
                {
                    m_Controllable.velocity = new Vector3(
                        leftStick.x * m_Controllable.speed,
                        m_Controllable.velocity.y,
                        leftStick.y * m_Controllable.speed);

                    float angle = m_Player.playerCamera.transform.eulerAngles.y * (Mathf.PI / 180);
                    angle += Mathf.Atan(m_Controllable.velocity.x / m_Controllable.velocity.z);

                    if ((m_Controllable.velocity.x < 0.0f && m_Controllable.velocity.z < 0.0f) ||
                        (m_Controllable.velocity.x > 0.0f && m_Controllable.velocity.z < 0.0f) ||
                        (m_Controllable.velocity.x == 0.0f && m_Controllable.velocity.z < 0.0f))
                        angle += Mathf.PI;

                    m_Controllable.velocity = new Vector3(
                        m_Controllable.speed * Mathf.Sin(angle),
                        0,
                        m_Controllable.speed * Mathf.Cos(angle));

                    m_Controllable.transform.eulerAngles = new Vector3(
                        m_Controllable.transform.eulerAngles.x,
                        angle * (180f / Mathf.PI),
                        m_Controllable.transform.eulerAngles.z);
                }
            }

            IUsesSkills skillUser = m_Controllable as IUsesSkills;

#if !UNITY_WEBGL
            bool[] isPressed = new bool[skillUser.skills.Count];
            for (int j = 0; j < skillUser.skills.Count; ++j)
            {
                isPressed[j] = GameManager.self.GetButtonState(
                    m_PlayerIndex,
                    KeyConfiguration.self.userConfigurations[m_PlayerIndex].skillButtons[j].keyCode);
            }
#else
            bool[] isPressed =
            {
                    Input.GetAxisRaw("Skill 1") != 0.0f,
                    Input.GetAxisRaw("Skill 2") != 0.0f,
                    Input.GetAxisRaw("Skill 3") != 0.0f,
                    Input.GetAxisRaw("Skill 4") != 0.0f
                };
#endif
            if (skillUser != null)
            {
                for (int j = 0; j < skillUser.skills.Count; ++j)
                {
                    if (Input.GetKey(KeyConfiguration.self.userConfigurations[m_PlayerIndex].skillKeys[j].keyCode) || isPressed[j])
                        Publisher.self.Broadcast(Event.UseSkill, skillUser, j);
                }
            }
        }

        #endregion

        public void Register(IControllable a_Controllable)
        {
            m_Controllable = a_Controllable.gameObject.GetComponent<Unit>();

            m_Controllable.movementFSM.AddTransition(MovementState.Init, MovementState.Idle);
            m_Controllable.movementFSM.AddTransition(MovementState.Idle, MovementState.Walking);
            m_Controllable.movementFSM.AddTransition(MovementState.Walking, MovementState.Running);

            m_Controllable.movementFSM.AddTransition(MovementState.Running, MovementState.Walking);
            m_Controllable.movementFSM.AddTransition(MovementState.Walking, MovementState.Idle);
        }
        public void Register(Player a_Player)
        {
            m_Player = a_Player;

            Register(m_Player.unit);
        }

        public void UnRegister(Player a_Player)
        {
            m_Player = null;
        }
        public void UnRegister(IControllable a_Controllable)
        {
            Destroy(gameObject);
        }
    }
}