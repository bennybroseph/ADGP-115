using System;
using Library;
using UI;
using Units.Controller;
using UnityEngine;
using Event = Define.Event;

namespace Units
{   //Public Enemy class that inherits IStats and IAttackable from interfaces 
    public class Enemy : Unit
    {
        [SerializeField]
        private UnitNameplate m_Nameplate;

        #region -- UNITY FUNCTIONS --
        void Awake()
        {
            if (m_Nameplate != null)
            {
                UnitNameplate nameplate = Instantiate(m_Nameplate);
                nameplate.parent = this;
                nameplate.Awake();
            }

            if (m_NavMeshAgent == null)
                m_NavMeshAgent = GetComponent<NavMeshAgent>();

            m_Health = m_MaxHealth;
            m_Mana = m_MaxMana;
            m_Defense = m_MaxDefense;

            SetController();

            m_DamageFSM = new FiniteStateMachine<DamageState>();

            m_DamageFSM.AddTransition(DamageState.Init, DamageState.Idle);
            m_DamageFSM.AddTransitionFromAny(DamageState.Dead);

            m_MovementFSM.Transition(MovementState.Idle);

            m_DamageFSM.Transition(DamageState.Idle);
        }

        protected void Start()
        {
            Publisher.self.Broadcast(Event.UnitInitialized, this);
        }

        protected void Update()
        {
            if (m_Health <= 0.0f)
                Destroy(gameObject);
        }

        protected void OnDestroy()
        {
            m_Controller.UnRegister(this);

            Publisher.self.Broadcast(Event.UnitDied, this);
        }
        #endregion

        protected void SetController()
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
    }
}
