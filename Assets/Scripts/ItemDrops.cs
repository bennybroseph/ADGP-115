using UnityEngine;
using System.Collections;
using Interfaces;
using System;

public class ItemDrops : MonoBehaviour, IChildable<IControllable>
{
    [SerializeField]
    private int m_HealthIncrease;
    [SerializeField]
    private int m_ManaIncrease;
    [SerializeField]
    private IControllable m_Parent;

    public int healthIncrease
    {
        get { return m_HealthIncrease; }
        set { m_HealthIncrease = value; }
    }

    public int manaIncrease
    {
        get { return m_ManaIncrease; }
        set { m_ManaIncrease = value; }
    }

    public IControllable parent
    {
        get { return m_Parent; }
        set { m_Parent = value; }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime * 5);
        transform.Translate(new Vector3(0, Mathf.Sin(Time.time * 9)) * Time.deltaTime, 0);

        if (m_Parent != null)
            transform.position = Vector3.Lerp(transform.position, m_Parent.transform.position, Time.deltaTime * 2);
    }

    void OnTriggerEnter(Collider a_Collision)
    {
        IControllable controllable = a_Collision.gameObject.GetComponent<IControllable>();
        if (controllable == null)
            return;

        IStats unit = controllable.gameObject.GetComponent<IStats>();

        if (gameObject.GetComponents<Collider>()[0].enabled == false)
        {
            if (controllable.GetHashCode() == m_Parent.GetHashCode())
            {
                if (unit.damageFSM.currentState == DamageState.Dead)
                    return;

                unit.health += m_HealthIncrease;
                unit.health = Mathf.Clamp(unit.health, 0, unit.maxHealth);

                unit.mana += m_ManaIncrease;
                unit.mana = Mathf.Clamp(unit.mana, 0, unit.maxMana);
                Destroy(gameObject);
            }
        }
        else if (controllable.controllerType == ControllerType.User &&
           (m_HealthIncrease != 0 && unit.health < unit.maxHealth ||
            m_ManaIncrease != 0 && unit.mana < unit.maxMana))
        {
            gameObject.GetComponents<Collider>()[0].enabled = false;
            m_Parent = controllable;
        }

    }
}
