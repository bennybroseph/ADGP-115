using Library;

namespace Interfaces
{
    #region -- ENUM --
    public enum DamageState
        {
            Init,
            Idle,
            TakingDamge,
            Dead,
        }
    #endregion

    #region -- INTERFACE --
    public interface IAttackable
        {
            // String Name property
            string unitName { get; }
            string unitNickname { get; set; }

            float maxHealth { get; }
            // Health property
            float health { get; set; }
            // Max Defense property
            float maxDefense { get; }
            // Defense property
            float defense { get; set; }
            string faction { get; set; }
            FiniteStateMachine<DamageState> damageFSM { get; }
        }
    #endregion
}
