using UnityEngine;
using System.Collections;
using Interfaces;
using Units.Skills;

public class Melee : BaseSkills, IMovable {

    #region -- VARIABLES --
    private Vector3 m_TotalVelocity;
    private Vector3 m_Velocity;
    [SerializeField]
    private float m_Speed;
    #endregion

    #region -- PROPERTIES --
    public Vector3 totalVelocity
    {
        get { return m_TotalVelocity; }
        set { m_TotalVelocity = value; }
    }
    public Vector3 velocity
    {
        get { return m_Velocity; }
        set { m_Velocity = value; }
    }

    public float speed
    {
        get { return m_Speed; }
        set { m_Speed = value; }
    }
    #endregion

    #region -- UNITY FUNCTIONS --
    // Use this for initialization
    private void Start()
    {

        if (m_Parent == null)
            return;

        Physics.IgnoreCollision(GetComponent<Collider>(), m_Parent.gameObject.GetComponent<Collider>());

        transform.SetParent(m_Parent.gameObject.transform, false);
        transform.position = m_Parent.gameObject.transform.position;
        transform.localPosition += new Vector3(0.5f,0,-0.5f);
        
        m_Velocity = new Vector3(
            Mathf.Cos((m_Parent.gameObject.transform.eulerAngles.y) * (Mathf.PI / 180)) * m_Speed,
            0,
            Mathf.Sin(-(m_Parent.gameObject.transform.eulerAngles.y) * (Mathf.PI / 180)) * m_Speed);
    }

    private void FixedUpdate()
    {
        //Move();
    }

    // Update is called once per frame
    private void Update()
    {
        m_CurrentLifetime += Time.deltaTime;

        if (m_CurrentLifetime >= m_MaxLifetime)
            Destroy(gameObject);
    }

    private void LateUpdate()
    {
        SetRotation();
    }
    #endregion

    private void SetRotation()
    {
        if (m_Velocity == Vector3.zero)
            return;

        float rotationY = 90 + Mathf.Atan(m_Velocity.x / m_Velocity.z) * (180.0f / Mathf.PI);

        if ((m_Velocity.x < 0.0f && m_Velocity.z < 0.0f) ||
            (m_Velocity.x > 0.0f && m_Velocity.z < 0.0f) ||
            (m_Velocity.x > 0.0f && m_Velocity.z == 0.0f))
            rotationY += 180;

        transform.rotation = Quaternion.Euler(new Vector3(90, rotationY, 0));
    }


    public void Move()
    {
        transform.position += (m_Velocity + m_TotalVelocity) * Time.deltaTime;
    }

    public override string UpdateDescription(Skill a_Skill)
    {
        string description = skillData.name + " is a physical skill that does " + a_Skill.skillData.damage + " damage!";
        return description;
    }

}
