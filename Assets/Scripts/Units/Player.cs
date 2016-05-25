// Unit class used for storing Player Data.
using Interfaces;
using UI;
using UnityEngine;

namespace Units
{
    // Public unit class that takes in IStats and IAttack
    public class Player : Unit, IChildable<UIManager>
    {
        #region -- VARIABLES --
        [SerializeField]
        private UIManager m_Parent;
        #endregion

        #region -- PROPERTIES --
        public UIManager parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }
        #endregion

        #region -- UNITY FUNCTIONS --
        // protected override void Awake() { base.Awake(); }
       
        protected override void Start()
        {
            base.Start();

            UIAnnouncer.self.DelayedAnnouncement(m_UnitNickname + ", Pick a skill below!", 1.0f);
            UIAnnouncer.self.DelayedAnnouncement(m_UnitNickname + " has entered the arena!", 1.5f);
        }

        // protected override void Update() { base.Update(); }
        protected override void LateUpdate()
        {
            base.LateUpdate();
            SetRotation();
        }
        // protected override void OnDestroy() { base.OnDestroy(); }
        #endregion

        private void SetRotation()
        {
            if (m_Velocity == Vector3.zero)
                return;

            float rotationY = Mathf.Atan(m_Velocity.x / m_Velocity.z) * (180.0f / Mathf.PI) - 90;

            if ((m_Velocity.x < 0.0f && m_Velocity.z < 0.0f) ||
                (m_Velocity.x > 0.0f && m_Velocity.z < 0.0f) ||
                (m_Velocity.x == 0.0f && m_Velocity.z < 0.0f))
                rotationY += 180;

            Vector3 rotation = new Vector3(
                0,
                rotationY,
                0);

            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}

