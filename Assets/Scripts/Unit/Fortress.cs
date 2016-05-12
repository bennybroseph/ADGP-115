using System;
using Library;
using UnityEngine;

using Event = Define.Event;

namespace Unit
{
//Currently Working on Fortress.
    public class Fortress : MonoBehaviour, IAttackable
    {
        private int m_Health = 100;

        private int m_Defense;

        private string m_Unitname;

        //Sets a private int for health = 100
        //private int m_FortHealth = 100;
        //Sets a private int for max health = 100
        private int m_MaxfortHealth = 100;

        private void Start()
        {
            //Empty for now
        
        }

        private void Update()
        {
            //If Fort Health greater than Fort Max health
            if (m_Health > m_MaxfortHealth)
            {
                //Set Fort Health to equal max fort health
                m_Health = m_MaxfortHealth;
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

        public int health
        {
            get { return m_Health;}
            set { m_Health = value; }
        }

        public int defense
        {
            get { return m_Defense;}
            set { m_Defense = value; }
        }

        public string unitName
        {
            get { return m_Unitname; }
            set { m_Unitname = value; }
        }

        public FiniteStateMachine<DamageState> damageFSM
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private void OnTriggerEnter(Collider a_Col)
        {
            //Will be implemented in the future.
            //if (a_Col == "")
            //{
                
            //}
            
        }




        //public void Fight()
        //{
        //    //Function used to detect when the base is attacked
        //    //The base health will decrease as enemies reach a certain point 
        //}
    }
}
