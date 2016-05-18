using System.CodeDom;
using UnityEngine;
using System.Collections;
using Interfaces;
using Units;

public class Goblin : Enemy
{
    private delegate void VoidFunction();

    private bool m_CoroutineIsRunning;

    void OnCollisionStay(Collision a_Collision)
    {
        if (a_Collision.transform.gameObject.tag == "Player")
        {
            IAttackable attackableObject = a_Collision.transform.gameObject.GetComponent<IAttackable>();
            if (attackableObject != null)
            {
                attackableObject.damageFSM.Transition(DamageState.TakingDamge);
                // If routine is not running
                if (!m_CoroutineIsRunning)
                    // Run it
                    StartCoroutine(AttackDelay(attackableObject));
            }
        }

    }

    private IEnumerator AttackDelay(IAttackable a_Attackable)
    {// Set to true
        m_CoroutineIsRunning = true;

        yield return StartCoroutine(
            WaitAndDoThis(
                0.5f,
                    delegate
                    {
                        Debug.Log("Taking damage");
                        a_Attackable.health -= 1;
                    }, 
                true));
        // When routine starts again set run to false
        m_CoroutineIsRunning = false;
    }

    private IEnumerator WaitAndDoThis(float a_TimeToWait, VoidFunction a_Delegate, bool a_CallDelegateFirst = false)
    {
        if (a_CallDelegateFirst)
            a_Delegate();

        yield return new WaitForSeconds(a_TimeToWait);

        if (!a_CallDelegateFirst)
            a_Delegate();
    }
}

