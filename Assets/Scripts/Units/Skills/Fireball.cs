using Interfaces;
using UnityEngine;
using UI;
using Units.Controller;

namespace Units.Skills
{
    public class Fireball : BaseSkills, IMovable
    {
        #region -- VARIABLES --
        private Vector3 m_TotalVelocity;
        private Vector3 m_Velocity;
        [SerializeField]
        private float m_Speed;

        [SerializeField]
        private bool m_IsDestroyed;
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

            AudioManager.self.PlaySound(SoundTypes.Fireball);
            Physics.IgnoreCollision(GetComponent<Collider>(), m_Parent.gameObject.GetComponent<Collider>());

            transform.position = m_Parent.gameObject.transform.position;

            m_Velocity = new Vector3(
                Mathf.Cos((m_Parent.gameObject.transform.eulerAngles.y - 90) * (Mathf.PI / 180)) * m_Speed,
                0,
                Mathf.Sin(-(m_Parent.gameObject.transform.eulerAngles.y - 90) * (Mathf.PI / 180)) * m_Speed);
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void LateUpdate()
        {
            SetRotation();
        }
        #endregion

        #region -- OTHER VOID FUNCTIONS
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

        private void OnTriggerEnter(Collider a_Collision)
        {
            if (m_IsDestroyed)
                return;

            IAttackable attackableObject = a_Collision.transform.gameObject.GetComponentInParent<IAttackable>();

            if (attackableObject != null && attackableObject.faction != m_Parent.faction)
            {
                attackableObject.damageFSM.Transition(DamageState.TakingDamge);

                attackableObject.health -= m_SkillData.damage;

                UIAnnouncer.self.FloatingText(
                    m_SkillData.damage,
                    a_Collision.transform.position,
                    FloatingTextType.MagicDamage);

                if (a_Collision.transform.GetComponent<IStats>() != null && attackableObject.health <= 0)
                    m_Parent.experience += a_Collision.transform.GetComponent<IStats>().experience;

                m_IsDestroyed = true;
                Destroy(gameObject);
            }
        }

        public void Move()
        {
            transform.position += (m_Velocity + m_TotalVelocity) * Time.deltaTime;
        }

        public override string UpdateDescription(Skill a_Skill)
        {
            string description = skillData.name + " is a magical skill" + "\nLevel: " + a_Skill.level + "\nDamage: " + a_Skill.skillData.damage +
            "\nCost: " + a_Skill.skillData.cost + "\nMaxCooldown: " + a_Skill.skillData.maxCooldown;
            return description;
        }
        #endregion
    }
}
