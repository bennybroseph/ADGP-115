using System;
using System.Collections.Generic;
using Interfaces;
using Library;
using UI;
using Units.Skills;
using Units.Controller;
using UnityEngine;
using Event = Define.Event;

namespace Units
{   //Public Enemy class that inherits IStats and IAttackable from interfaces 
    public class Enemy : Unit
    {
        #region -- VARIABLES --
        [SerializeField]
        private UnitNameplate m_Nameplate;
        #endregion

        #region -- UNITY FUNCTIONS --
        protected virtual void Awake()
        {
            if (m_Nameplate != null)
            {
                UnitNameplate nameplate = Instantiate(m_Nameplate);
                nameplate.parent = this;
                nameplate.Awake();
            }

            if (m_Skills == null)
                m_Skills = new List<Skill>();
            else
                for (int i = 0; i < m_Skills.Count; i++)
                {
                    m_Skills[i].skillIndex = i;
                    m_Skills[i].parent = this;
                }

            if (m_NavMeshAgent == null)
                m_NavMeshAgent = GetComponent<NavMeshAgent>();

            m_Level = 1;
            m_Health = m_MaxHealth;
            m_Mana = m_MaxMana;
            m_Defense = m_MaxDefense;

            SetController();

            m_DamageFSM = new FiniteStateMachine<DamageState>();

            m_DamageFSM.AddTransition(DamageState.Init, DamageState.Idle);
            m_DamageFSM.AddTransitionFromAny(DamageState.Dead);

            m_MovementFSM.Transition(MovementState.Idle);

            m_DamageFSM.Transition(DamageState.Idle);

            Publisher.self.Subscribe(Event.UseSkill, OnUseSkill);
        }

        protected void Start()
        {
            Publisher.self.Broadcast(Event.UnitInitialized, this);
        }

        protected void Update()
        {
            foreach (Skill skill in m_Skills)
                skill.UpdateCooldown();

            if (m_Health <= 0.0f)
                Destroy(gameObject);
        }

        protected void OnDestroy()
        {
            m_Controller.UnRegister(this);

            Publisher.self.UnSubscribe(Event.UseSkill, OnUseSkill);
            Publisher.self.Broadcast(Event.UnitDied, this);
            
        }
        #endregion

        #region -- PROTECTED FUNCTIONS --
        private void SetController()
        {
            m_MovementFSM = new FiniteStateMachine<MovementState>();

            switch (m_ControllerType)
            {
                case ControllerType.GoblinMage:
                    m_Controller = AIController.self;
                    break;
                case ControllerType.Goblin:
                    m_Controller = AIController.self;
                    break;
                case ControllerType.Fortress:
                    m_Controller = UserController.self;
                    break;
                case ControllerType.User:
                    m_Controller = UserController.self;
                    break;
            }

            m_Controller.Register(this);
        }

        private void OnUseSkill(Event a_Event, params object[] a_Params)
        {
            IUsesSkills unit = a_Params[0] as IUsesSkills;
            int skillIndex = (int)a_Params[1];

            if (unit.GetHashCode() != this.GetHashCode() ||
                m_Skills.Count <= skillIndex ||
                !(m_Skills[skillIndex].remainingCooldown <= 0.0f) ||
                !(m_Skills[skillIndex].skillData.cost <= m_Mana))
                return;

            GameObject newObject = Instantiate(m_Skills[skillIndex].skillPrefab);

            Physics.IgnoreCollision(GetComponent<Collider>(), newObject.GetComponent<Collider>());

            newObject.transform.position = transform.position;
            newObject.GetComponent<IChildable<IUsesSkills>>().parent = this;

            newObject.GetComponent<IMovable>().velocity = new Vector3(
                Mathf.Cos((-transform.eulerAngles.y + 90) * (Mathf.PI / 180)) * newObject.GetComponent<IMovable>().speed,
                0,
                Mathf.Sin((-transform.eulerAngles.y + 90) * (Mathf.PI / 180)) * newObject.GetComponent<IMovable>().speed);

            newObject.GetComponent<ICastable<IUsesSkills>>().skillData = m_Skills[skillIndex].skillData;

            mana -= m_Skills[skillIndex].skillData.cost;

            m_Skills[skillIndex].PutOnCooldown();
        }

    }
    #endregion
}
