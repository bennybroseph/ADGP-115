using Library;
using UnityEngine;
using UnityEngine.SceneManagement;

using Event = Define.Event;

namespace Unit
{
//Currently Working on Fortress.
    public class Fortress : MonoBehaviour
    {
        //Sets a private int for health = 100
        private int m_FortHealth = 100;
        //Sets a private int for max health = 100
        private int m_MaxfortHealth = 100;

        private void Start()
        {
            //Empty for now
        
        }

        private void Update()
        {
            //If Fort Health greater than Fort Max health
            if (m_FortHealth > m_MaxfortHealth)
            {
                //Set Fort Health to equal max fort health
                m_FortHealth = m_MaxfortHealth;
            }
            //If Fort Helth is less or equal to zero
            if (m_FortHealth <= 0)
            {
                //If forthealth = 0
                m_FortHealth = 0;
                //Broadcast the Event GameOver
                Publisher.self.Broadcast(Event.GameOver);

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
