// Unit class used for storing Player and Enemy Data.
using System.Collections.Generic;

using UI;
using UnityEngine;

#if !UNITY_WEBGL
using XInputDotNetPure;
#endif

using Library;
using Units.Controller;
using Units.Skills;
using Event = Define.Event;
using System;

namespace Units
{
    // Public unit class that takes in IStats and IAttack
    public class Player : Unit, IChildable<UIManager>
    {
        #region -- VARIABLES --
        // Private member variables
        [SerializeField]
        private UnitNameplate m_Nameplate;
  
        [SerializeField]
        private UIManager m_Parent;

        [SerializeField]
        private Vector3 m_CurrentRotation;
        [SerializeField]
        private Vector3 m_OriginalRotation;
        #endregion

        #region -- PROPERTIES --
        // Mana/Currency int property
        public UIManager parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }
        #endregion

        #region -- UNITY FUNCTIONS --

        private void Awake()
        {
            if (m_Nameplate != null)
            {
                UnitNameplate nameplate = Instantiate(m_Nameplate);
                nameplate.parent = this;
            }

            Publisher.self.Subscribe(Event.UseSkill, OnUseSkill);
        }

        private void Start()
        {
            if (m_Skills == null)
                m_Skills = new List<Skill>();
            else
                for (int i = 0; i < m_Skills.Count; i++)
                {
                    m_Skills[i].skillIndex = i;
                    m_Skills[i].parent = this;
                }

            GetComponent<NavMeshAgent>().updateRotation = false;

            m_OriginalRotation = transform.eulerAngles;
            m_CurrentRotation = m_OriginalRotation;

            SetController();

            m_Health = m_MaxHealth;
            m_Mana = m_MaxMana;
            m_Defense = m_MaxDefense;

            Publisher.self.Broadcast(Event.UnitInitialized, this);
        }

        private void FixedUpdate()
        {

        }

        private void Update()
        {
            foreach (Skill skill in m_Skills)
                skill.UpdateCooldown();
        }

        public void LateUpdate()
        {
            SetRotation();
            SetMovementFSM();
        }

        private void OnDestroy()
        {
            m_Controller.UnRegister(this);
            Publisher.self.Broadcast(Event.UnitDied, this);
        }
        #endregion

        #region -- OTHER PRIVATE FUNCTIONS --
        private void SetController()
        {
            m_MovementFSM = new FiniteStateMachine<MovementState>();

            switch (m_ControllerType)
            {
                default:
                    m_Controller = UserController.self;
                    m_CanMoveWithInput = true;
                    m_Controller.Register(this);
                    break;
            }

            m_DamageFSM = new FiniteStateMachine<DamageState>();

            m_DamageFSM.AddTransition(DamageState.Init, DamageState.Idle);
            m_DamageFSM.AddTransitionFromAny(DamageState.Dead);

            m_MovementFSM.Transition(MovementState.Idle);

            m_DamageFSM.Transition(DamageState.Idle);
        }

        private void SetRotation()
        {
            if (m_Velocity == Vector3.zero)
                return;

            float rotationY = (Mathf.Atan(m_Velocity.x / m_Velocity.z) * (180.0f / Mathf.PI)) - 90;

            if ((m_Velocity.x < 0.0f && m_Velocity.z < 0.0f) ||
                (m_Velocity.x > 0.0f && m_Velocity.z < 0.0f) ||
                (m_Velocity.x == 0.0f && m_Velocity.z < 0.0f))
                rotationY += 180;

            m_CurrentRotation = new Vector3(
                m_OriginalRotation.x,
                rotationY,
                m_OriginalRotation.z);

            transform.rotation = Quaternion.Euler(m_CurrentRotation);
        }

        private void SetMovementFSM()
        {
            if (m_Velocity == Vector3.zero)
                m_MovementFSM.Transition(MovementState.Idle);
            else
                m_MovementFSM.Transition(MovementState.Walking);

            if (m_Velocity.magnitude >= m_Speed / 2.0f)
                m_MovementFSM.Transition(MovementState.Running);
        }

        private void OnUseSkill(Event a_Event, params object[] a_Params)
        {
            IUsesSkills unit = a_Params[0] as IUsesSkills;
            int skillIndex = (int)a_Params[1];

            if ((Player)unit != this ||
                m_Skills.Count <= skillIndex ||
                !(m_Skills[skillIndex].remainingCooldown <= 0.0f) ||
                !(m_Skills[skillIndex].skillData.cost <= m_Mana))
                return;
            
            GameObject newObject = Instantiate(m_Skills[skillIndex].skillPrefab);

            Physics.IgnoreCollision(GetComponent<Collider>(), newObject.GetComponent<Collider>());

            newObject.transform.position = transform.position;
            newObject.GetComponent<IChildable<IUsesSkills>>().parent = this;

            newObject.GetComponent<IMovable>().velocity = new Vector3(
                Mathf.Cos((-m_CurrentRotation.y) * (Mathf.PI / 180)) * newObject.GetComponent<IMovable>().speed,
                0,
                Mathf.Sin((-m_CurrentRotation.y) * (Mathf.PI / 180)) * newObject.GetComponent<IMovable>().speed);

            newObject.GetComponent<ICastable<IUsesSkills>>().skillData = m_Skills[skillIndex].skillData;

            mana -= m_Skills[skillIndex].skillData.cost;

            m_Skills[skillIndex].PutOnCooldown();
        }
        #endregion
    }
}

