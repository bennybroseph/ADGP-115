using UnityEngine;
using System.Collections;
using Interfaces;
using UI;
using Units;

public class Goblin : Enemy
{
    private delegate void VoidFunction();

    private bool m_CoroutineIsRunning;

    void OnCollisionStay(Collision a_Collision)
    {
        if (a_Collision.transform.gameObject.tag == "Player")
        {
            IAttackable attackableObject = a_Collision.gameObject.GetComponent<IAttackable>();
            if (attackableObject != null)
            {
                attackableObject.damageFSM.Transition(DamageState.TakingDamge);
                // If routine is not running
                if (!m_CoroutineIsRunning)
                    // Run it
                    StartCoroutine(AttackDelay(attackableObject, a_Collision.transform.position));
            }
        }
    }

    private IEnumerator AttackDelay(IAttackable a_Attackable, Vector3 a_Position)
    {
        // Set to true
        m_CoroutineIsRunning = true;

        yield return StartCoroutine(
            WaitAndDoThis(
                0.5f,
                    delegate
                    {
                        IStats goblin = gameObject.GetComponent<IStats>();
                        a_Attackable.health -= 1;
                        UIAnnouncer.self.FloatingText(1, a_Position, FloatingTextType.PhysicalDamage);
                        if (a_Attackable.health <= 0)
                        {goblin.experience += 100;}
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

