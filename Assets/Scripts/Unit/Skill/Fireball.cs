using System;
using UnityEngine;

namespace Unit.Skill
{
    public class Fireball : MonoBehaviour, IControlable, IParentable
    {
        [SerializeField]
        private float m_CurrentLifetime;
        [SerializeField]
        private float m_MaxLifetime;

        [SerializeField]
        private Vector3 m_Velocity;
        [SerializeField]
        private Moving m_IsMoving;

        [SerializeField]
        private GameObject m_Parent;

        public Moving isMoving
        {
            get { return m_IsMoving; }
            set { m_IsMoving = value; }
        }
        public Vector3 velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }
        public bool canMoveWithInput { get; set; }

        public GameObject parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        // Use this for initialization
        private void Start() { }

        private void FixedUpdate()
        {
            Move();
            transform.rotation = Quaternion.identity;
        }

        // Update is called once per frame
        private void Update()
        {
            m_CurrentLifetime += Time.deltaTime;

            if (m_CurrentLifetime >= m_MaxLifetime)
                Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision a_Collision)
        {
            if (a_Collision.transform.gameObject != m_Parent)
            {
                IAttackable attackableObject = a_Collision.transform.gameObject.GetComponent<IAttackable>();
                if (attackableObject != null)
                    Debug.Log("Hit " + attackableObject.unitName);
            }
        }

        public void Move()
        {
            transform.position += m_Velocity;
        }
    }
}
