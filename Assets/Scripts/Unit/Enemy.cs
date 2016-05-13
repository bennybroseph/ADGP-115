
using System;
using Library;
using UI;
using UnityEngine;
using Event = Define.Event;

namespace Unit
{   //Public Enemy class that inherits IStats and IAttackable from interfaces 
    public class Enemy : MonoBehaviour, IStats
    {
        [SerializeField]
        private UnitNameplate m_Nameplate;
        //Private int and string memorable variables
        [SerializeField]
        private string m_UnitName;
        [SerializeField]
        private string m_UnitNickname;
        [SerializeField]
        private float m_MaxHealth;
        [SerializeField]
        private float m_Health;
        [SerializeField]
        private float m_MaxDefense;
        [SerializeField]
        private float m_Defense;
        [SerializeField]
        private float m_MaxMana;
        [SerializeField]
        private float m_Mana;
        [SerializeField]
        private float m_Experience;  //Total experience each monster drops
        [SerializeField]
        private int m_Level; //wont be displayed for Enemy
        [SerializeField]
        private float m_Speed;


        private Pathfinding m_Pathfinding;

        #region -- PROPERTIES --
        //Public string Name property
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
        //Public int health property
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
        //Public int Defense property
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
        //Public int Mana property
        public float mana
        {
            get { return m_Mana; }
            set { m_Mana = value; Publisher.self.DelayedBroadcast(Event.UnitManaChanged, this); }
        }
        //Public int Experience property
        public float experience
        {
            get { return m_Experience; }
            set { m_Experience = value; }
        }
        //Public int Level property
        public int level
        {
            get { return m_Level; }
            set { m_Level = value; Publisher.self.DelayedBroadcast(Event.UnitLevelChanged, this); }
        }
        //Public int Speed property
        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }

        public FiniteStateMachine<DamageState> damageFSM
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        void Awake()
        {
            if (m_Nameplate != null)
            {
                UnitNameplate nameplate = Instantiate(m_Nameplate);
                nameplate.parent = gameObject;
                nameplate.Awake();
            }

        }

        void Start()
        {
            m_Health = m_MaxHealth;
            m_Mana = m_MaxMana;
            m_Defense = m_MaxDefense;

            if (m_Pathfinding == null)
            {
                m_Pathfinding = new Pathfinding(
                    GameObject.FindGameObjectWithTag("Fortress"),
                    GetComponent<NavMeshAgent>());
            }

            Publisher.self.Broadcast(Event.UnitInitialized, this);
        }

        void Update()
        {
            m_Pathfinding.Search();
        }
    }

}
