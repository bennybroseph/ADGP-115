using UI;
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
        // protected override void Awake() { base.Awake(); }

        protected override void Start()
        {
            base.Start();

            if (Random.value * 10 >= 7)
                UIAnnouncer.self.Chat(m_UnitNickname, m_Taunt, transform.position);
        }

        // protected override void Update() { base.Update(); }
        // protected override void LateUpdate() { base.LateUpdate(); }
        // protected override void OnDestroy() { base.OnDestroy(); }
        #endregion
    }
}
