using UnityEngine;
using System.Collections.Generic;
using Interfaces;
using Library;
using UI;
using Units.Controller;
using Units.Skills;

using Event = Define.Event;

namespace Units
{
    public class Unit : MonoBehaviour, IUsesSkills, IControllable
    {
        #region -- VARIABLES --
        // Private member variables
        [SerializeField]
        private UnitNameplate m_Nameplate = null;

        [SerializeField]
        protected ControllerType m_ControllerType;
        protected IController m_Controller;
        protected NavMeshAgent m_NavMeshAgent;
        protected GameObject m_Following;

        protected FiniteStateMachine<DamageState> m_DamageFSM;
        protected FiniteStateMachine<MovementState> m_MovementFSM;

        [SerializeField, ReadOnly]
        protected List<Skill> m_Skills;
        [SerializeField]
        protected List<GameObject> m_SkillPrefabs;

        protected int m_StoredSkillUpgrades = 0;

        [SerializeField]
        protected string m_UnitName;
        [SerializeField]
        protected string m_UnitNickname;

        [SerializeField]
        protected string m_Faction;

        [SerializeField]
        protected float m_BaseHealth = 5f;
        [SerializeField]
        protected float m_HealthGrowth = 2.5f;
        protected float m_MaxHealth;
        protected float m_Health;

        [SerializeField]
        protected float m_BaseMana = 2f;
        [SerializeField]
        protected float m_ManaGrowth = 1f;
        protected float m_MaxMana;
        protected float m_Mana;

        [SerializeField]
        protected float m_BaseDefense = 2f;
        [SerializeField]
        protected float m_DefenseGrowth = 1f;
        protected float m_MaxDefense;
        protected float m_Defense;

        [SerializeField]
        protected int m_BaseLevel;
        protected int m_Level;
        protected float m_Experience;

        protected Vector3 m_TotalVelocity;
        protected Vector3 m_Velocity;

        [SerializeField]
        protected float m_BaseSpeed = 5f;
        [SerializeField]
        protected float m_SpeedGrowth = 0.2f;
        protected float m_Speed;
        protected Moving m_IsMoving;

        protected bool m_CanMoveWithInput;
        #endregion

        #region -- PROPERTIES --
        public NavMeshAgent navMashAgent
        {
            get { return m_NavMeshAgent; }
            set { m_NavMeshAgent = value; }
        }

        public GameObject following
        {
            get { return m_Following; }
            set { m_Following = value; }
        }

        public IController controller
        {
            get { return m_Controller; }
            set { m_Controller = value; }
        }

        public ControllerType controllerType
        {
            get { return m_ControllerType; }
            set { m_ControllerType = value; }
        }

        public FiniteStateMachine<DamageState> damageFSM
        {
            get { return m_DamageFSM; }
            set { m_DamageFSM = value; }
        }

        public FiniteStateMachine<MovementState> movementFSM
        {
            get { return m_MovementFSM; }
            set { m_MovementFSM = value; }
        }

        public List<Skill> skills
        {
            get { return m_Skills; }
            set { m_Skills = value; }
        }

        //string name property
        public string unitName
        {
            get { return m_UnitName; }
            set { m_UnitName = value; }
        }
        //string nickname property
        public string unitNickname
        {
            get { return m_UnitNickname; }
            set { m_UnitNickname = value; }
        }
        public string faction
        {
            get { return m_Faction; }
            set { m_Faction = value; }
        }
        //max mana int property
        public float maxMana
        {
            get { return m_MaxMana; }
            set { m_MaxMana = value; Publisher.self.DelayedBroadcast(Event.UnitMaxManaChanged, this); }
        }

        //mana int property
        public float mana
        {
            get { return m_Mana; }
            set { m_Mana = value; Publisher.self.DelayedBroadcast(Event.UnitManaChanged, this); }
        }

        //Max defense int property
        public float maxDefense
        {
            get { return m_MaxDefense; }
            set { m_MaxDefense = value; }
        }
        //Defense int property
        public float defense
        {
            get { return m_Defense; }
            set { m_Defense = value; }
        }

        //maxhealth int property
        public float maxHealth
        {
            get { return m_MaxHealth; }
            set { m_MaxHealth = value; Publisher.self.DelayedBroadcast(Event.UnitMaxHealthChanged, this); }
        }

        //health int property
        public float health
        {
            get { return m_Health; }
            set { m_Health = value; Publisher.self.DelayedBroadcast(Event.UnitHealthChanged, this); }
        }
        //Experience int property
        public float experience
        {
            get { return m_Experience; }
            set
            {
                m_Experience = value;
                Publisher.self.DelayedBroadcast(Event.UnitEXPChanged, this);
                SetLevel();
            }
        }

        //Level int property
        public int level
        {
            get { return m_Level; }
            private set { m_Level = value; Publisher.self.DelayedBroadcast(Event.UnitLevelChanged, this); }
        }

        //totalVelecotiy Vector3 property 
        public Vector3 totalVelocity
        {
            get { return m_TotalVelocity; }
            set { m_TotalVelocity = value; }
        }
        //Vector
        public Vector3 velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }


        //Speed int property
        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }

        public Moving isMoving
        {
            get { return m_IsMoving; }
            set { m_IsMoving = value; }
        }

        //canMoveWithInput bool property
        public bool canMoveWithInput
        {
            get { return m_CanMoveWithInput; }
            set { m_CanMoveWithInput = value; }
        }

        public List<GameObject> baseSkills
        {
            get { return m_SkillPrefabs; }
        }
        #endregion

        #region -- UNITY FUNCTIONS --
        protected virtual void Awake()
        {
            if (m_NavMeshAgent == null)
                m_NavMeshAgent = GetComponent<NavMeshAgent>();

            if (m_Nameplate != null)
            {
                UnitNameplate nameplate = Instantiate(m_Nameplate);
                nameplate.parent = this;
            }

            if (m_Skills == null)
                m_Skills = new List<Skill>();
            else
            {
                foreach (GameObject skillPrefab in m_SkillPrefabs)
                {
                    if (skillPrefab.GetComponent<ICastable<IUsesSkills>>() == null)
                        m_SkillPrefabs.Remove(skillPrefab);
                }
                for (int i = 0, j = 0; i < m_SkillPrefabs.Count; ++i)
                {
                    m_Skills.Add(new Skill());
                    m_Skills[j].skillIndex = j;
                    m_Skills[j].skillData = m_SkillPrefabs[i].GetComponent<ICastable<IUsesSkills>>().skillData.Clone();
                    m_Skills[j].skillData.skillIndex = j;
                    m_Skills[j].skillPrefab = m_SkillPrefabs[i].gameObject;
                    m_Skills[j].parent = this;
                    ICastable<IUsesSkills> castable = m_SkillPrefabs[j].GetComponent<ICastable<IUsesSkills>>();
                    ++j;
                }
            }

            m_CanMoveWithInput = true;
            m_MovementFSM = new FiniteStateMachine<MovementState>();

            m_DamageFSM = new FiniteStateMachine<DamageState>();

            m_DamageFSM.AddTransition(DamageState.Init, DamageState.Idle);
            m_DamageFSM.AddTransitionFromAny(DamageState.Dead);

            m_MovementFSM.Transition(MovementState.Idle);

            m_DamageFSM.Transition(DamageState.Idle);

            Publisher.self.Subscribe(Event.UpgradeSkill, OnUpgradeSkill);
            Publisher.self.Subscribe(Event.UseSkill, OnUseSkill);
        }

        protected virtual void Start()
        {
            SetLevel(m_BaseLevel);

            m_MaxHealth = m_BaseHealth;
            m_Health = m_MaxHealth;

            m_MaxMana = m_BaseMana;
            m_Mana = m_MaxMana;

            m_MaxDefense = m_BaseDefense;
            m_Defense = m_MaxDefense;

            Publisher.self.Broadcast(Event.UnitInitialized, this);
        }

        protected virtual void Update()
        {
            foreach (Skill skill in m_Skills)
                skill.UpdateCooldown();

            if (m_Health <= 0.0f)
                Destroy(gameObject);
        }

        protected virtual void LateUpdate()
        {
            SetMovementFSM();
        }

        protected virtual void OnDestroy()
        {
            if (m_Controller != null)
                m_Controller.UnRegister(this);

            Publisher.self.UnSubscribe(Event.UseSkill, OnUseSkill);
            Publisher.self.UnSubscribe(Event.UpgradeSkill, OnUpgradeSkill);
            Publisher.self.Broadcast(Event.UnitDied, this);
        }
        #endregion

        #region -- PRIVATE FUNCTIONS --
        public void GetLevelBar(out float a_LastLevel, out float a_NextLevel)
        {
            a_LastLevel = ReverseLevelAlgorithm(m_Level);
            a_NextLevel = ReverseLevelAlgorithm(m_Level + 1);
        }
        /// <summary> Use this function to set the unit's level. Do NOT set it manually </summary>
        /// <param name="a_Level"></param>
        private void SetLevel(int a_Level)
        {
            m_Experience = ReverseLevelAlgorithm(a_Level);
            SetLevel();
        }
        /// <summary> Gets called automatically whenever 'experience' gets changed </summary>
        private void SetLevel()
        {
            int oldLevel = m_Level;
            int newLevel = LevelAlgorithm(m_Experience);

            if (newLevel == m_Level)
                return;

            float oldMaxHealth = maxHealth;
            float oldMaxMana = maxMana;

            maxHealth = m_BaseHealth + (m_HealthGrowth * m_Level);
            maxMana = m_BaseMana + (m_ManaGrowth * m_Level);
            maxDefense = m_BaseDefense + (m_DefenseGrowth * m_Level);
            speed = m_BaseSpeed + (m_SpeedGrowth * m_Level);

            health += maxHealth - oldMaxHealth;
            mana += maxMana - oldMaxMana;

            level = newLevel;

            if (oldLevel < m_Level)
            {
                m_StoredSkillUpgrades += m_Level - oldLevel;
                Publisher.self.Broadcast(Event.UnitLevelUp, this);
                Publisher.self.Broadcast(Event.UnitCanUpgradeSkill, this);

                if (oldLevel > 0)
                    UIAnnouncer.self.FloatingText("Level Up!", transform.position, FloatingTextType.Overhead);
            }
        }

        private int LevelAlgorithm(float a_Experience)
        {
            int newLevel;

            newLevel = (int)Mathf.Sqrt(a_Experience / 10f);

            return newLevel;
        }
        private float ReverseLevelAlgorithm(int a_Level)
        {
            float newExperience;

            newExperience = Mathf.Pow(a_Level, 2) * 10;

            return newExperience;
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
        #endregion

        #region -- EVENTS --
        private void OnUseSkill(Event a_Event, params object[] a_Params)
        {
            IUsesSkills unit = a_Params[0] as IUsesSkills;
            if (unit == null)
                return;

            int skillIndex = (int)a_Params[1];

            if (unit.GetHashCode() != GetHashCode() ||
                m_Skills.Count <= skillIndex ||
                m_Skills[skillIndex].level <= 0 ||
                !(m_Skills[skillIndex].remainingCooldown <= 0.0f) ||
                !(m_Skills[skillIndex].skillData.cost <= m_Mana))
                return;

            GameObject newObject = Instantiate(m_Skills[skillIndex].skillPrefab);
            ICastable<IUsesSkills> newSkill = newObject.GetComponent<ICastable<IUsesSkills>>();

            newSkill.parent = this;
            newSkill.skillData = m_Skills[skillIndex].skillData.Clone();

            if (m_Skills[skillIndex].skillData.cost != 0f)
                mana -= m_Skills[skillIndex].skillData.cost;

            m_Skills[skillIndex].PutOnCooldown();
        }

        private void OnUpgradeSkill(Event a_Event, params object[] a_Params)
        {
            IUsesSkills unit = a_Params[0] as IUsesSkills;
            int skillIndex = (int)a_Params[1];

            if (unit == null || unit.GetHashCode() != GetHashCode() || m_SkillPrefabs.Count == 0)
                return;

            Skill skill = unit.skills[skillIndex];

            ICastable<IUsesSkills> castable = m_SkillPrefabs[skillIndex].GetComponent<ICastable<IUsesSkills>>();

            ++skill.level;

            skill.skillData = m_SkillPrefabs[skillIndex].GetComponent<BaseSkill>().GetSkillData(skill.level);

            --m_StoredSkillUpgrades;
            if (m_StoredSkillUpgrades != 0)
                Publisher.self.DelayedBroadcast(Event.UnitCanUpgradeSkill, this);
        }

        #endregion
    }
}
