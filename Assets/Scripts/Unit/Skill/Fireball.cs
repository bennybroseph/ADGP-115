using System;
using Library;
using UnityEngine;

namespace Unit.Skill
{
    public class Fireball : MonoBehaviour, IMovable, ICastable
    {
        #region -- VARIABLES --
        [SerializeField]
        private float m_CurrentLifetime;
        [SerializeField]
        private float m_MaxLifetime;

        [SerializeField]
        private Vector3 m_TotalVelocity;
        [SerializeField]
        private Vector3 m_Velocity;
        [SerializeField]
        private float m_Speed;

        [SerializeField]
        private Moving m_IsMoving;

        [SerializeField]
        private GameObject m_Parent;

        [SerializeField]
        private Vector3 m_CurrentRotation;
        [SerializeField]
        private Vector3 m_OriginalRotation;
        #endregion

        #region -- PROPERTIES --
        public float currentLifetime
        {
            get { return m_CurrentLifetime; }
        }

        public float maxLifetime
        {
            get { return m_MaxLifetime; }
            set { m_MaxLifetime = value; }
        }

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

        public Moving isMoving
        {
            get { return m_IsMoving; }
            set { m_IsMoving = value; }
        }
        public bool canMoveWithInput { get; set; }

        public GameObject parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }
        #endregion

        // Use this for initialization
        private void Start()
        {
            m_OriginalRotation = transform.eulerAngles;
            m_CurrentRotation = m_OriginalRotation;
        }

        private void FixedUpdate()
        {
            Move();
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
            CalculateRotation();
        }
        private void CalculateRotation()
        {
            if (m_Velocity == Vector3.zero)
                return;

            float rotationY = 90 + (Mathf.Atan(m_Velocity.x / m_Velocity.z) * (180.0f / Mathf.PI));

            if ((m_Velocity.x < 0.0f && m_Velocity.z < 0.0f) ||
                (m_Velocity.x > 0.0f && m_Velocity.z < 0.0f) ||
                (m_Velocity.x == 0.0f && m_Velocity.z < 0.0f))
                rotationY += 180;

            m_CurrentRotation = new Vector3(
                m_OriginalRotation.x,
                rotationY,
                m_OriginalRotation.z);

            transform.rotation = Quaternion.Euler(m_CurrentRotation);
        }

        private void OnCollisionEnter(Collision a_Collision)
        {
            if (a_Collision.transform.gameObject != m_Parent)
            {
                IAttackable attackableObject = a_Collision.transform.gameObject.GetComponent<IAttackable>();
                if (attackableObject != null)
                {
                    attackableObject.damageFSM.Transition(DamageState.TakingDamge);
                    Debug.Log("Hit " + attackableObject.unitName);
                }
            }
        }

        public void Move()
        {
            transform.position += (m_Velocity + m_TotalVelocity) * Time.deltaTime;
        }
    }
}
