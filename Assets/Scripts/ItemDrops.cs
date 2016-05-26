using UnityEngine;
using Interfaces;

public class ItemDrops : MonoBehaviour, IChildable<IControllable>
{
    [SerializeField]
    private float m_HealthIncrease;
    [SerializeField]
    private float m_ManaIncrease;
    [SerializeField]
    private IControllable m_Parent;
    private float m_Speed = 0;

    public float healthIncrease
    {
        get { return m_HealthIncrease; }
        set { m_HealthIncrease = value; }
    }

    public float manaIncrease
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
        {
            m_Speed += Time.deltaTime * 2;
            GetComponent<Rigidbody>().velocity = new Vector3(
                (m_Parent.transform.position.x - transform.position.x) * m_Speed,
                0,
                (m_Parent.transform.position.z - transform.position.z) * m_Speed);
        }
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

                if ((m_HealthIncrease != 0 && unit.health < unit.maxHealth) ||
                    (m_ManaIncrease != 0 && unit.mana < unit.maxMana))
                {
                    unit.health += m_HealthIncrease * unit.maxHealth;
                    unit.health = Mathf.Clamp(unit.health, 0, unit.maxHealth);
                
                    unit.mana += m_ManaIncrease;
                    unit.mana = Mathf.Clamp(unit.mana, 0, unit.maxMana);
                    Destroy(gameObject);
                }
                else
                {
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    gameObject.GetComponents<Collider>()[0].enabled = true;
                    m_Parent = null;
                }
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
