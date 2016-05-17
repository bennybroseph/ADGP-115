using UnityEngine;
using System.Collections;
using Library;

namespace Interfaces
{
        public enum DamageState
        {
            Init,
            Idle,
            TakingDamge,
            Dead,
        }

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

            FiniteStateMachine<DamageState> damageFSM { get; }
        }
}
