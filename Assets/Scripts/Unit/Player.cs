// Unit class used for storing Player and Enemy Data.

using System;
using System.Collections.Generic;
using Library;
using UI;
using UnityEngine;
using XInputDotNetPure;
using Event = Define.Event;

namespace Unit
{
    // Public unit class that takes in IStats and IAttack
    public class Player : MonoBehaviour, IUsesSkills, IControlable, IParentable<UIManager>
    {
        #region -- VARIABLES --
        // Private int and string memorable variables
        [SerializeField]
        private UnitNameplate m_Nameplate;

        [SerializeField]
        private FiniteStateMachine<MovementState> m_MovementFSM;
        [SerializeField]
        private FiniteStateMachine<DamageState> m_DamageFSM;

        [SerializeField]
        private List<SkillData> m_Skills;

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

        public List<SkillData> skills
        {
            get { return m_Skills; }
            set { m_Skills = value; }
        }

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
                nameplate.parent = gameObject;
                nameplate.Awake();
            }

            Publisher.self.Subscribe(Event.UseSkill, OnUseSkill);
        }

        private void Start()
        {
            if (m_Skills == null)
                m_Skills = new List<SkillData>();

            m_OriginalRotation = transform.eulerAngles;
            m_CurrentRotation = m_OriginalRotation;

            InitFSM();

            m_Health = m_MaxHealth;
            m_Mana = m_MaxMana;
            m_Defense = m_MaxDefense;

            Publisher.self.Broadcast(Event.UnitInitialized, this);
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Update()
        {
            for (int i = 0; i < m_Skills.Count; ++i)
            {
                if (m_Skills[i].remainingCooldown != 0.0f)
                {
                    float remainingCooldown = m_Skills[i].remainingCooldown - Time.deltaTime;
                    if (remainingCooldown < 0.0f)
                        remainingCooldown = 0.0f;

                    m_Skills[i] = new SkillData(
                        m_Skills[i].skillPrefab,
                        m_Skills[i].cooldown,
                        remainingCooldown,
                        m_Skills[i].cost);

                    Publisher.self.Broadcast(Event.SkillCooldownChanged, this, i);
                }
            }

            //m_Velocity = Vector3.zero;

            if (m_CanMoveWithInput)
            {
                m_IsMoving.forward = Input.GetKey(KeyCode.W) | (GameManager.self.state.DPad.Up == ButtonState.Pressed);
                m_IsMoving.back = Input.GetKey(KeyCode.S) | (GameManager.self.state.DPad.Down == ButtonState.Pressed);
                m_IsMoving.left = Input.GetKey(KeyCode.A) | (GameManager.self.state.DPad.Left == ButtonState.Pressed);
                m_IsMoving.right = Input.GetKey(KeyCode.D) | (GameManager.self.state.DPad.Right == ButtonState.Pressed);

                if (m_IsMoving.forward)
                    m_Velocity = new Vector3(0, m_Velocity.y, m_Speed);
                if (m_IsMoving.back)
                    m_Velocity = new Vector3(0, m_Velocity.y, -m_Speed);
                if (m_IsMoving.left)
                    m_Velocity = new Vector3(-m_Speed, m_Velocity.y, 0);
                if (m_IsMoving.right)
                    m_Velocity = new Vector3(m_Speed, m_Velocity.y, 0);

                if (m_IsMoving.forward && m_IsMoving.left)
                    m_Velocity = new Vector3(-Mathf.Sqrt(m_Speed * 2), m_Velocity.y, Mathf.Sqrt(m_Speed * 2));
                if (m_IsMoving.forward && m_IsMoving.right)
                    m_Velocity = new Vector3(Mathf.Sqrt(m_Speed * 2), m_Velocity.y, Mathf.Sqrt(m_Speed * 2));
                if (m_IsMoving.back && m_IsMoving.left)
                    m_Velocity = new Vector3(-Mathf.Sqrt(m_Speed * 2), m_Velocity.y, -Mathf.Sqrt(m_Speed * 2));
                if (m_IsMoving.back && m_IsMoving.right)
                    m_Velocity = new Vector3(Mathf.Sqrt(m_Speed * 2), m_Velocity.y, -Mathf.Sqrt(m_Speed * 2));

                if (GameManager.self.state.ThumbSticks.Left.X != 0.0f ||
                    GameManager.self.state.ThumbSticks.Left.Y != 0.0f)
                {
                    m_Velocity = new Vector3(
                        GameManager.self.state.ThumbSticks.Left.X * m_Speed,
                        m_Velocity.y,
                        GameManager.self.state.ThumbSticks.Left.Y * m_Speed);
                }
            }

            if (Input.GetKeyDown(KeyCode.Q) || GameManager.self.state.Triggers.Right > 0.0f)
                Publisher.self.Broadcast(Event.UseSkill, 1);
            if (Input.GetKeyDown(KeyCode.E) || GameManager.self.state.Triggers.Left > 0.0f)
                Publisher.self.Broadcast(Event.UseSkill, 2);
        }

        public void LateUpdate()
        {
            SetRotation();
            SetMovementFSM();
        }
        #endregion

        #region -- OTHER PRIVATE FUNCTIONS --
        private void InitFSM()
        {
            m_MovementFSM = new FiniteStateMachine<MovementState>();

            m_MovementFSM.AddTransition(MovementState.Init, MovementState.Idle);
            m_MovementFSM.AddTransition(MovementState.Idle, MovementState.Walking);
            m_MovementFSM.AddTransition(MovementState.Walking, MovementState.Running);

            m_MovementFSM.AddTransition(MovementState.Running, MovementState.Walking);
            m_MovementFSM.AddTransition(MovementState.Walking, MovementState.Idle);

            m_DamageFSM = new FiniteStateMachine<DamageState>();

            m_DamageFSM.AddTransition(DamageState.Init, DamageState.Idle);
            m_DamageFSM.AddTransitionFromAny(DamageState.Dead);

            m_MovementFSM.Transition(MovementState.Idle);

            m_DamageFSM.Transition(DamageState.Idle);
        }

        public void Move()
        {
            transform.position += (m_Velocity + m_TotalVelocity) * Time.deltaTime;

            if (m_Velocity != Vector3.zero &&
               (m_IsMoving == Moving.Nowhere ||
                    (GameManager.self.state.ThumbSticks.Left.X == 0.0f &&
                     GameManager.self.state.ThumbSticks.Left.Y == 0.0f)))
            {
                Movable.Brake(this);
            }
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
            int skillIndex = (int)a_Params[0];

            if (m_Skills.Count >= skillIndex &&
                m_Skills[skillIndex - 1].remainingCooldown <= 0.0f &&
                m_Skills[skillIndex - 1].cost <= m_Mana)
            {
                Debug.Log("Use Skill " + skillIndex);

                skillIndex -= 1;

                GameObject newObject = Instantiate(m_Skills[skillIndex].skillPrefab);

                newObject.transform.position = transform.position;

                newObject.GetComponent<IParentable<GameObject>>().parent = gameObject;

                newObject.GetComponent<IMovable>().velocity = new Vector3(
                    Mathf.Cos((-m_CurrentRotation.y) * (Mathf.PI / 180)) * newObject.GetComponent<IMovable>().speed,
                    0,
                    Mathf.Sin((-m_CurrentRotation.y) * (Mathf.PI / 180)) * newObject.GetComponent<IMovable>().speed);

                mana -= m_Skills[skillIndex].cost;

                m_Skills[skillIndex] = new SkillData(
                    m_Skills[skillIndex].skillPrefab,
                    m_Skills[skillIndex].cooldown,
                    m_Skills[skillIndex].cooldown,
                    m_Skills[skillIndex].cost);
            }
        }
        #endregion
    }
}

