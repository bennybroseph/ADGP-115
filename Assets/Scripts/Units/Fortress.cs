using System;
using Library;
using UnityEngine;

using Event = Define.Event;

namespace Units
{
    //Currently Working on Fortress.
    public class Fortress : MonoBehaviour, IAttackable
    {
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
            set { m_Health = value; }
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
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        private void Start()
        {
            //Empty for now
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
                //Broadcast the Event GameOver
                Publisher.self.Broadcast(Event.GameOver);

            }
        }
    }
}
