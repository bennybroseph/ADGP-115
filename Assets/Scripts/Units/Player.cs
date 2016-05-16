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
    public class Player : MonoBehaviour, IUsesSkills, IControlable, IChildable<UIManager>
    {
        #region -- VARIABLES --
        // Private member variables
        [SerializeField]
        private UnitNameplate m_Nameplate;

        private NavMeshAgent m_NaveMeshAgent;
        private GameObject m_following;

        [SerializeField]
        private ControllerType m_ControllerType;
        [SerializeField]
        private IController m_Controller;

        [SerializeField]
        private FiniteStateMachine<MovementState> m_MovementFSM;
        [SerializeField]
        private FiniteStateMachine<DamageState> m_DamageFSM;
        
        [SerializeField]
        private List<Skill> m_Skills;

        [SerializeField]
        private string m_UnitName;
        [SerializeField]
        private string m_UnitNickname;

        [SerializeField]
        private float m_MaxHealth;
        [SerializeField]
        private float m_Health;
        [SerializeField]
        private float m_MaxMana;
        [SerializeField]
        private float m_Mana;
        [SerializeField]
        private float m_MaxDefense;
        [SerializeField]
        private float m_Defense;

        [SerializeField]
        private float m_Exp;
        [SerializeField]
        private int m_Level;

        [SerializeField]
        private Vector3 m_TotalVelocity;
        [SerializeField]
        private Vector3 m_Velocity;
        [SerializeField]
        private float m_Speed;

        [SerializeField]
        private Moving m_IsMoving;

        [SerializeField]
        private bool m_CanMoveWithInput;

        [SerializeField]
        private UIManager m_Parent;

        [SerializeField]
        private Vector3 m_CurrentRotation;
        [SerializeField]
        private Vector3 m_OriginalRotation;
        #endregion

        #region -- PROPERTIES --
        public ControllerType controllerType
        {
            get { return m_ControllerType; }
            set { m_ControllerType = value; }
        }
        public IController controller
        {
            get { return m_Controller; }
            set { m_Controller = value; }
        }

        public FiniteStateMachine<MovementState> movementFSM
        {
            get { return m_MovementFSM; }
            private set { m_MovementFSM = value; }
        }
        public FiniteStateMachine<DamageState> damageFSM
        {
            get { return m_DamageFSM; }
            private set { m_DamageFSM = value; }
        }

        // String name property
        public string unitName
        {
            get { return m_UnitName; }
            private set { m_UnitName = value; }
        }

        public string unitNickname
        {
            get { return m_UnitNickname; }
            set { m_UnitNickname = value; }
        }

        public float maxHealth
        {
            get { return m_MaxHealth; }
            private set { m_MaxHealth = value; }
        }
        // Health int property
        public float health
        {
            get { return m_Health; }
            set { m_Health = value; Publisher.self.DelayedBroadcast(Event.UnitHealthChanged, this); }
        }

        public float maxDefense
        {
            get { return m_MaxDefense; }
            private set { m_MaxDefense = value; }
        }
        // Defense int property
        public float defense
        {
            get { return m_Defense; }
            set { m_Defense = value; }
        }

        public float maxMana
        {
            get { return m_MaxMana; }
            private set { m_MaxMana = value; }
        }
        // Mana/Currency int property
        public float mana
        {
            get { return m_Mana; }
            set { m_Mana = value; Publisher.self.DelayedBroadcast(Event.UnitManaChanged, this); }
        }
        // Experience int property
        public float experience
        {
            get { return m_Exp; }
            set { m_Exp = value; }
        }
        // Speed int property
        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }
        // Level int property
        public int level
        {
            get { return m_Level; }
            private set { m_Level = value; Publisher.self.DelayedBroadcast(Event.UnitLevelChanged, this); }
        }

        public Moving isMoving
        {
            get { return m_IsMoving; }
            set { m_IsMoving = value; }
        }

        public Vector3 totalVelocity
        {
            get { return m_TotalVelocity; }
            set { m_TotalVelocity = value; }
        }
        public Vector3 velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }

        public bool canMoveWithInput
        {
            get { return m_CanMoveWithInput; }
            set { m_CanMoveWithInput = value; }
        }

        public List<Skill> skills
        {
            get { return m_Skills; }
            private set { m_Skills = value; }
        }

        public UIManager parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        public NavMeshAgent navMashAgent
        {
            get
            {
                return m_NaveMeshAgent;
            }
        }

        public GameObject following
        {
            get
            {
                return m_following;
            }

            set
            {
                m_following = value;
            }
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

