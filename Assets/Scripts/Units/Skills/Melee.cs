using UnityEngine;
using System.Collections;
using Interfaces;
using Units.Skills;

public class Melee : BaseSkills
{

    #region -- VARIABLES --
    [SerializeField]
    private float m_CurrentAngle;
    #endregion

    #region -- PROPERTIES --

    #endregion

    #region -- UNITY FUNCTIONS --
    // Use this for initialization
    private void Start()
    {

        if (m_Parent == null)
            return;

        Physics.IgnoreCollision(GetComponentInChildren<Collider>(), m_Parent.gameObject.GetComponent<Collider>());

        transform.SetParent(m_Parent.gameObject.transform, false);
        transform.localPosition += new Vector3(
            0,
            0,
            0.5f);

        //m_Velocity = new Vector3(
        //    Mathf.Cos((m_Parent.gameObject.transform.eulerAngles.y) * (Mathf.PI / 180)) * m_Speed,
        //    0,
        //    Mathf.Sin(-(m_Parent.gameObject.transform.eulerAngles.y) * (Mathf.PI / 180)) * m_Speed);
    }

    private void FixedUpdate()
    {

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
        transform.position = new Vector3(
            Mathf.Cos(transform.eulerAngles.y * (Mathf.PI / 180)) * -0.5f,
            0,
            Mathf.Sin(-transform.eulerAngles.y * (Mathf.PI / 180)) * -0.5f);
    }

    public override string UpdateDescription(Skill a_Skill)
    {
        string description = skillData.name + " is a physical skill that does " + a_Skill.skillData.damage + " damage!";
        return description;
    }

}
