
using System;
using Library;
using UnityEngine;

namespace Unit
{   //Public Enemy class that inherits IStats and IAttackable from interfaces 
    public class Enemy : MonoBehaviour, IStats
    {
        //Private int and string memorable variables
        private int m_Health;
        private int m_Defense;
        private int m_Exp;  //Total experience each monster drops
        private int m_Lvl; //wont be displayed for Enemy
        private float m_Speed;
        private int m_Mana;
        private string m_UnitName;
        private Pathfinding m_Pathfinding;

        #region -- PROPERTIES --
        //Public int health property
        public int health
        {
            get { return m_Health; }
            set { m_Health = value; }
        }
        //Public int Defense property
        public int defense
        {
            get { return m_Defense; }
            set { m_Defense = value; }
        }
        //Public int Experience property
        public int experience
        {
            get { return m_Exp; }
            set { m_Exp = value; }
        }
        //Public int Level property
        public int level
        {
            get { return m_Lvl; }
            set { m_Lvl = value; }
        }
        //Public int Speed property
        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }
        //Public int Mana property
        public int mana
        {
            get { return m_Mana; }
            set { m_Mana = value; }
        }
        //Public string Name property
        public string unitName
        {
            get { return m_UnitName; }
            set { m_UnitName = value; }
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
