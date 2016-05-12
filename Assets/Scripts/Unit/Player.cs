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
        private int m_Speed;

        [SerializeField]
        private Moving m_IsMoving;
        [SerializeField]
        private Vector3 m_Velocity;

        [SerializeField]
        private bool m_CanMoveWithInput;

        [SerializeField]
        private GameManager m_GameManager;

        [SerializeField]
        private Vector3 m_CurrentRotation;
        [SerializeField]
        private Vector3 m_OriginalRotation;
        #endregion

        #region -- PROPERTIES --
        // Health int property
        public int health
        {
            get { return m_Health; }
            set { m_Health = value; }
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
        public int speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }
        // Level int property
        public int level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
        // Mana/Currency int property
        public int mana
        {
            get { return m_Mana; }
            set { m_Mana = value; }
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

        // Unit class that stores Health, Defense, Exp, Level, Speed, Mana, Name
        private void Start()
        {
            if (m_Skills == null)
                m_Skills = new List<SkillData>();
            if (m_GameManager == null)
                m_GameManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>();

            m_OriginalRotation = transform.eulerAngles;
            m_CurrentRotation = m_OriginalRotation;

            Publisher.self.Subscribe(Event.UseSkill, OnUseSkill);
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Update()
        {
            for (int i = 0; i < m_Skills.Count; ++i)
            {
                float remainingCooldown = m_Skills[i].remainingCooldown - Time.deltaTime;
                if (remainingCooldown < 0.0f)
                    remainingCooldown = 0.0f;

                m_Skills[i] = new SkillData(
                    m_Skills[i].skillPrefab, 
                    m_Skills[i].cooldown, 
                    remainingCooldown);
            }

            m_Velocity = Vector3.zero;

            if (m_CanMoveWithInput)
            {
                m_IsMoving.up = Input.GetKey(KeyCode.W) | (m_GameManager.state.DPad.Up == ButtonState.Pressed);
                m_IsMoving.down = Input.GetKey(KeyCode.S) | (m_GameManager.state.DPad.Down == ButtonState.Pressed);
                m_IsMoving.left = Input.GetKey(KeyCode.A) | (m_GameManager.state.DPad.Left == ButtonState.Pressed);
                m_IsMoving.right = Input.GetKey(KeyCode.D) | (m_GameManager.state.DPad.Right == ButtonState.Pressed);

                if (m_IsMoving.up)
                    m_Velocity += Vector3.up * m_Speed;
                if (m_IsMoving.down)
                    m_Velocity += Vector3.down * m_Speed;
                if (m_IsMoving.left)
                    m_Velocity += Vector3.left * m_Speed;
                if (m_IsMoving.right)
                    m_Velocity += Vector3.right * m_Speed;

                if (m_GameManager.state.ThumbSticks.Left.X != 0.0f ||
                    m_GameManager.state.ThumbSticks.Left.Y != 0.0f)
                {
                    m_Velocity = new Vector3(
                        m_GameManager.state.ThumbSticks.Left.X * m_Speed,
                        m_GameManager.state.ThumbSticks.Left.Y * m_Speed);
                }
            }

            if (Input.GetKeyDown(KeyCode.Q) || m_GameManager.state.Triggers.Right > 0.0f)
                Publisher.self.Broadcast(Event.UseSkill, 1);
            if (Input.GetKeyDown(KeyCode.E) || m_GameManager.state.Triggers.Left > 0.0f)
                Publisher.self.Broadcast(Event.UseSkill, 2);
        }

        public void Move()
        {
            transform.position += m_Velocity * Time.deltaTime;
        }

        public void LateUpdate()
        {
            CalculateRotation();
        }

        private void CalculateRotation()
        {
            if (m_Velocity == Vector3.zero)
                return;

            float rotationX = -(Mathf.Atan(m_Velocity.x / m_Velocity.y) * (180.0f / Mathf.PI));

            if ((m_Velocity.x < 0.0f && m_Velocity.y < 0.0f) ||
                (m_Velocity.x > 0.0f && m_Velocity.y < 0.0f) ||
                (m_Velocity.x == 0.0f && m_Velocity.y < 0.0f))
                rotationX += 180;

            m_CurrentRotation = new Vector3(
                rotationX,
                m_OriginalRotation.y,
                m_OriginalRotation.z);

            transform.rotation = Quaternion.Euler(m_CurrentRotation);
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

                newObject.GetComponent<IControlable>().velocity = new Vector3(
                    Mathf.Cos((m_CurrentRotation.x + 90) * (Mathf.PI / 180)),
                    Mathf.Sin((m_CurrentRotation.x + 90) * (Mathf.PI / 180)),
                    0);

                m_Skills[skillIndex] = new SkillData(
                    m_Skills[skillIndex].skillPrefab, 
                    m_Skills[skillIndex].cooldown, 
                    m_Skills[skillIndex].cooldown);
            }
        }
    }
}

