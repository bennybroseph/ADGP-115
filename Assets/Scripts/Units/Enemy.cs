using Interfaces;
using Library;
using UI;
using Units.Controller;
using UnityEngine;

namespace Units
{   //Public Enemy class that inherits IStats and IAttackable from interfaces 
    public class Enemy : Unit
    {
        #region -- VARIABLES --
        [SerializeField]
        protected string m_Taunt = "Do you fear death?";
        #endregion

        #region -- UNITY FUNCTIONS --
        protected override void Awake()
        {
            base.Awake();
            SetController();
        }

        protected override void Start()
        {
            base.Start();
            if (Random.value > 0.875)
                UIAnnouncer.self.Chat(m_UnitNickname, m_Taunt, this);
        }

        // protected override void Update() { base.Update(); }
        // protected override void LateUpdate() { base.LateUpdate(); }
        // protected override void OnDestroy() { base.OnDestroy(); }
        #endregion

        private void SetController()
        {
            switch (m_ControllerType)
            {
                case ControllerType.GoblinMage:
                    m_Controller = AIController.self;
                    break;
                case ControllerType.Goblin:
                    m_Controller = AIController.self;
                    break;
                case ControllerType.Fortress:
                    m_Controller = new GameObject().AddComponent<UserControllerInput>();
                    break;
                case ControllerType.User:
                    m_CanMoveWithInput = true;
                    m_Controller = new GameObject().AddComponent<UserControllerInput>();
                    break;
            }

            m_Controller.Register(this);
        }
    }
}
