using Interfaces;
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
        private UnitNameplate m_NameplatePrefab = null;

        [SerializeField]
        private string m_UnitName;
        [SerializeField]
        private string m_UnitNickname;

        [SerializeField]
        private string m_Faction;

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
            set { m_Health = value; Publisher.self.DelayedBroadcast(Event.FortressHealthChanged, this); }
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

        public string faction
        {
            get { return m_Faction; }
            set { m_Faction = value; }
        }
        #endregion

        private void Awake()
        {
            m_DamageFSM = new FiniteStateMachine<DamageState>();
            //Checks to see if NameplatePRefab not equal to null
            if (m_NameplatePrefab != null)
            {
                UnitNameplate nameplate = Instantiate(m_NameplatePrefab);
                nameplate.fortressParent = this;
            }
        }

        private void Start()
        {
            m_Health = m_MaxHealth;
            //Broadcast the FortressInitialized event in the console
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
            //If Fort Health is less or equal to zero
            if (m_Health <= 0)
            {
                //If forthealth = 0
                m_Health = 0;
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {   //Broadcasts the fortressdied event in the console.
            Publisher.self.Broadcast(Event.FortressDied, this);
        }
    }
}
