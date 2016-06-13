// Unit class used for storing Player Data.
using Interfaces;
using UI;
using Units.Controller;
using UnityEngine;

namespace Units
{
    // Public unit class that takes in IStats and IAttack
    public class Player : MonoBehaviour
    {
        #region -- VARIABLES --
        [Header("Prefabs")]
        [SerializeField]
        private Unit m_PlayerPrefab;
        [SerializeField]
        private ThirdPersonCamera m_CameraPrefab;
        [SerializeField]
        private UserControllerInput m_UserControllerInput;

        [Space]
        [SerializeField]
        private Unit m_Unit;
        [SerializeField]
        private IController m_Controller;
        [SerializeField]
        private ThirdPersonCamera m_PlayerCamera;
        #endregion

        #region -- PROPERTIES --
        public Unit unit
        {
            get { return m_Unit; }
        }

        public ThirdPersonCamera playerCamera
        {
            get { return m_PlayerCamera; }
        }
        #endregion

        #region -- UNITY FUNCTIONS --
        private void Awake()
        {
            m_Unit = Instantiate(m_PlayerPrefab);
            m_Unit.canMoveWithInput = true;

            m_PlayerCamera = Instantiate(m_CameraPrefab);
            m_PlayerCamera.following = m_Unit.gameObject;

            m_Controller = Instantiate(m_UserControllerInput);
            m_Controller.Register(this);

            UIAnnouncer.self.DelayedAnnouncement(m_Unit.unitNickname + ", Pick a skill below!", 1.0f);
            UIAnnouncer.self.DelayedAnnouncement(m_Unit.unitNickname + " has entered the arena!", 1.5f);
        }
        #endregion
    }
}

