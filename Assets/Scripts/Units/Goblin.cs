using System.CodeDom;
using UnityEngine;
using System.Collections;
using Interfaces;
using Units;

public class Goblin : Enemy {

    void OnCollisionEnter(Collision a_Collision)
    {
        if (a_Collision.transform.gameObject == GameObject.FindGameObjectWithTag("Player"))
        {
            IAttackable attackableObject = a_Collision.transform.gameObject.GetComponent<IAttackable>();
            if (attackableObject != null)
            {
                attackableObject.damageFSM.Transition(DamageState.TakingDamge);
                Debug.Log("Hit " + attackableObject.unitName);
                attackableObject.health -= 1;
            }
        }

    }
}
	
