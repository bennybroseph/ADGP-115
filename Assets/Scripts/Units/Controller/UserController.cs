using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using UnityEngine;

#if !UNITY_WEBGL
using XInputDotNetPure;
#endif

using Library;

using Event = Define.Event;

namespace Units.Controller
{
    public sealed class UserController : MonoSingleton<UserController>, IController
    {
        [SerializeField]
        private List<IControlable> m_Controlables;

        public UserController()
        {
            m_Controlables = new List<IControlable>();
        }

        #region -- UNITY FUNCTIONS --
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
                if (i >= KeyConfiguration.self.userConfigurations.Count)
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
                        forward = Input.GetKey(KeyConfiguration.self.userConfigurations[i].verticalKeyAxis.positive.keyCode) | dPad.forward,
                        back = Input.GetKey(KeyConfiguration.self.userConfigurations[i].verticalKeyAxis.negative.keyCode) | dPad.back,
                        left = Input.GetKey(KeyConfiguration.self.userConfigurations[i].horizontalKeyAxis.negative.keyCode) | dPad.left,
                        right = Input.GetKey(KeyConfiguration.self.userConfigurations[i].horizontalKeyAxis.positive.keyCode) | dPad.right
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

#if !UNITY_WEBGL
                bool[] isPressed =
                {
                    GameManager.self.state.Triggers.Right > 0.0f,
                    GameManager.self.state.Triggers.Left > 0.0f
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
                    if (Input.GetKeyDown(KeyConfiguration.self.userConfigurations[i].skillKeyCodes[0].keyCode) || isPressed[0])
                        Publisher.self.Broadcast(Event.UseSkill, skillUser, 0);
                    if (Input.GetKeyDown(KeyConfiguration.self.userConfigurations[i].skillKeyCodes[1].keyCode) || isPressed[1])
                        Publisher.self.Broadcast(Event.UseSkill, skillUser, 1);
                }

                i++;
            }
        }
        #endregion

        public void Register(IControlable a_Controlable)
        {
            m_Controlables.Add(a_Controlable);

            a_Controlable.movementFSM.AddTransition(MovementState.Init, MovementState.Idle);
            a_Controlable.movementFSM.AddTransition(MovementState.Idle, MovementState.Walking);
            a_Controlable.movementFSM.AddTransition(MovementState.Walking, MovementState.Running);

            a_Controlable.movementFSM.AddTransition(MovementState.Running, MovementState.Walking);
            a_Controlable.movementFSM.AddTransition(MovementState.Walking, MovementState.Idle);
        }
        public void UnRegister(IControlable a_Controlable)
        {
            m_Controlables.Remove(a_Controlable);
        }
    }
}