// Unit class used for storing Player and Enemy Data.

using System;
using System.Collections.Generic;
using Library;
using UnityEngine;
using XInputDotNetPure;
using Event = Define.Event;

namespace Unit
{
    // Public unit class that takes in IStats and IAttack
    public class Player : MonoBehaviour, IUsesSkills, IControlable
    {
        #region -- VARIABLES --
        // Private int and string memorable variables
        [SerializeField]
        private FiniteStateMachine<MovementState> m_MovementFSM;
        [SerializeField]
        private FiniteStateMachine<DamageState> m_DamageFSM;

        [SerializeField]
        private List<SkillData> m_Skills;

        [SerializeField]
        private string m_UnitName;

        [SerializeField]
        private int m_Health;
        [SerializeField]
        private int m_Mana;
        [SerializeField]
        private int m_Defense;

        [SerializeField]
        private int m_Exp;
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
        private Vector3 m_CurrentRotation;
        [SerializeField]
        private Vector3 m_OriginalRotation;
        #endregion

        #region -- PROPERTIES --
        public FiniteStateMachine<MovementState> movementFSM
        {
            get { return m_MovementFSM; }
        }
        public FiniteStateMachine<DamageState> damageFSM
        {
            get { return m_DamageFSM; }
        }

        // Health int property
        public int health
        {
            get { return m_Health; }
            set { m_Health = value; Publisher.self.DelayedBroadcast(Event.UnitHealthChanged, this); }
        }

        // Defense int property
        public int defense
        {
            get { return m_Defense; }
            set { m_Defense = value; }
        }
        // Experience int property
        public int experience
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
            set { m_Level = value; Publisher.self.DelayedBroadcast(Event.UnitLevelChanged, this); }
        }
        // Mana/Currency int property
        public int mana
        {
            get { return m_Mana; }
            set { m_Mana = value; Publisher.self.DelayedBroadcast(Event.UnitManaChanged, this); }
        }

        // String name property
        public string unitName
        {
            get { return m_UnitName; }
            set { m_UnitName = value; }
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
        #endregion

        #region -- UNITY FUNCTIONS --
        // Unit class that stores Health, Defense, Exp, Level, Speed, Mana, Name
        private void Start()
        {
            if (m_Skills == null)
                m_Skills = new List<SkillData>();

            m_OriginalRotation = transform.eulerAngles;
            m_CurrentRotation = m_OriginalRotation;

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

            Publisher.self.Subscribe(Event.UseSkill, OnUseSkill);
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Update()
        {
            health = 100;
            level = 1;
            mana = 100;

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
                        remainingCooldown);

                    Publisher.self.Broadcast(Event.SkillCooldownChanged, i, m_Skills[i].remainingCooldown);
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

            if (m_Skills.Count >= skillIndex && m_Skills[skillIndex - 1].remainingCooldown <= 0.0f)
            {
                Debug.Log("Use Skill " + skillIndex);

                skillIndex -= 1;

                GameObject newObject = Instantiate(m_Skills[skillIndex].skillPrefab);

                newObject.transform.position = transform.position;

                newObject.GetComponent<IParentable>().parent = gameObject;

                newObject.GetComponent<IMovable>().velocity = new Vector3(
                    Mathf.Cos((-m_CurrentRotation.y) * (Mathf.PI / 180)) * newObject.GetComponent<IMovable>().speed,
                    0,
                    Mathf.Sin((-m_CurrentRotation.y) * (Mathf.PI / 180)) * newObject.GetComponent<IMovable>().speed);

                m_Skills[skillIndex] = new SkillData(
                    m_Skills[skillIndex].skillPrefab,
                    m_Skills[skillIndex].cooldown,
                    m_Skills[skillIndex].cooldown);
            }
        }
    }
}

