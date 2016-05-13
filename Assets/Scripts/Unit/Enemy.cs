
using System;
using Library;
using UnityEngine;

namespace Unit
{   //Public Enemy class that inherits IStats and IAttackable from interfaces 
    public class Enemy : MonoBehaviour, IStats
    {
        //Private int and string memorable variables
        private string m_UnitName;
        private string m_UnitNickname;

        private float m_MaxHealth;
        private float m_Health;
        private float m_MaxDefense;
        private float m_Defense;

        private float m_MaxMana;
        private float m_Mana;
        private float m_Experience;  //Total experience each monster drops
        private int m_Level; //wont be displayed for Enemy
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
            set { m_Health = value; }
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
            set { m_Mana = value; }
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
            set { m_Level = value; }
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

        void Start()
        {
            if (m_Pathfinding == null)
            {
                m_Pathfinding = new Pathfinding(
                    GameObject.FindGameObjectWithTag("Player"),
                    GetComponent<NavMeshAgent>());
            }
        }

        void Update()
        {
            m_Pathfinding.Search();
        }
    }

}
