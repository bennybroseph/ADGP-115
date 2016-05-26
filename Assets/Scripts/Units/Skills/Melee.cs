using UnityEngine;
using System.Collections.Generic;
using Interfaces;
using UI;
using Units.Skills;

public class Melee : BaseSkills
{

    #region -- VARIABLES --
    [SerializeField]
    private float m_CurrentAngle;

    private List<IAttackable> m_HitUnits;
    #endregion

    #region -- PROPERTIES --

    #endregion

    #region -- UNITY FUNCTIONS --
    private void Awake()
    {
        m_HitUnits = new List<IAttackable>();
    }

    // Use this for initialization
    private void Start()
    {
        if (m_Parent == null)
            return;

        Physics.IgnoreCollision(GetComponentInChildren<Collider>(), m_Parent.gameObject.GetComponent<Collider>());
        transform.SetParent(m_Parent.gameObject.transform, false);
    }

    // Update is called once per frame
    private void Update()
    {
        m_CurrentLifetime += Time.deltaTime;

        transform.eulerAngles += new Vector3(
            0,
            (180f / m_MaxLifetime) * Time.deltaTime);

        if (m_CurrentLifetime >= m_MaxLifetime)
            Destroy(gameObject);
    }

    private void LateUpdate()
    {
        SetPosition();
    }
    #endregion

    private void SetPosition()
    {
        transform.localPosition = new Vector3(
            Mathf.Sin(transform.eulerAngles.y * (Mathf.PI / 180)) * 0.1f,
            transform.localPosition.y,
            Mathf.Cos(transform.eulerAngles.y * (Mathf.PI / 180)) * 0.1f);
    }

    public override string UpdateDescription(Skill a_Skill)
    {
        string description = skillData.name + " is a physical skill that does " + a_Skill.skillData.damage + " damage!";
        return description;
    }

    private void OnTriggerEnter(Collider a_Collision)
    {
        IAttackable attackableObject = a_Collision.transform.gameObject.GetComponent<IAttackable>();

        if (attackableObject != null && !m_HitUnits.Contains(attackableObject) && attackableObject.faction != m_Parent.faction)
        {
            m_HitUnits.Add(attackableObject);

            attackableObject.damageFSM.Transition(DamageState.TakingDamge);

            attackableObject.health -= m_SkillData.damage;

            UIAnnouncer.self.FloatingText(
                m_SkillData.damage,
                a_Collision.transform.position,
                FloatingTextType.PhysicalDamage);

            if (a_Collision.transform.GetComponent<IStats>() != null && attackableObject.health <= 0)
                m_Parent.experience += a_Collision.transform.GetComponent<IStats>().experience;
        }
    }

}
