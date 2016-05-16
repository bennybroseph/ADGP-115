using System;
using Library;
using UI;
using UnityEngine;

using Event = Define.Event;

namespace Units
{
    //Currently Working on Fortress.
    public class Fortress : MonoBehaviour, IAttackable, IParentable
    {
        [SerializeField]
        private FortressNameplate m_NameplatePrefab;

        [SerializeField]
        private string m_UnitName;
        [SerializeField]
        private string m_UnitNickname;

        [SerializeField]
        private float m_MaxHealth;
        [SerializeField, ReadOnly]
        private float m_Health;

        [SerializeField]
        private float m_MaxDefense;
        [SerializeField, ReadOnly]
        private float m_Defense;

        private FiniteStateMachine<DamageState> m_DamageFSM;

        #region -- PROPERTIES --
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
        public float health
        {
            get { return m_Health; }
            set { m_Health = value; Publisher.self.DelayedBroadcast(Event.FortressHealthChanged, this);}
        }

        public float maxDefense
        {
            get { return m_MaxDefense; }
            private set { m_MaxDefense = value; }
        }
        public float defense
        {
            get { return m_Defense; }
            set { m_Defense = value; }
        }

        public FiniteStateMachine<DamageState> damageFSM
        {
            get { return m_DamageFSM; }
        }
        #endregion

        private void Awake()
        {
            m_DamageFSM = new FiniteStateMachine<DamageState>();

            if (m_NameplatePrefab != null)
            {
                FortressNameplate nameplate = Instantiate(m_NameplatePrefab);
                nameplate.parent = this;
            }
        }

        private void Start()
        {
            m_Health = m_MaxHealth;

            Publisher.self.Broadcast(Event.FortressInitialized, this);
        }

        private void Update()
        {
            //If Fort Health greater than Fort Max health
            if (m_Health > m_MaxHealth)
            {
                //Set Fort Health to equal max fort health
                m_Health = m_MaxHealth;
            }
            //If Fort Helth is less or equal to zero
            if (m_Health <= 0)
            {
                //If forthealth = 0
                m_Health = 0;
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            Publisher.self.Broadcast(Event.FortressDied, this);
        }
    }
}
