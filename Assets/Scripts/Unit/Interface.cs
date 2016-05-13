// Interface

using System;
using System.Collections.Generic;
using Library;
using UnityEngine;

namespace Unit
{
    [Serializable]
    public struct Moving
    {
        public bool forward;
        public bool back;
        public bool left;
        public bool right;

        public static Moving nowhere = new Moving(false, false, false, false);

        public Moving(bool a_Forward, bool a_Back, bool a_Left, bool a_Right) : this()
        {
            forward = a_Forward;
            back = a_Back;
            left = a_Left;
            right = a_Right;
        }

        public static bool operator ==(Moving a_Left, Moving a_Right)
        {
            if (a_Left.forward == a_Right.forward &&
                a_Left.back == a_Right.back &&
                a_Left.left == a_Right.left &&
                a_Left.right == a_Right.right)
                return true;
            return false;
        }

        public static bool operator !=(Moving a_Left, Moving a_Right)
        {
            return !(a_Left == a_Right);
        }
    }

    public enum MovementState
    {
        Init,
        Idle,
        Walking,
        Running,
    }

    /// <summary>
    ///  Ensures the object can move using input or other stimulus 
    /// </summary>
    public interface IControlable : IMovable
    {
        /// <summary> Which directions the object is moving in. </summary>
        Moving isMoving { get; set; }
        bool canMoveWithInput { get; set; }

        FiniteStateMachine<MovementState> movementFSM { get; }

        void Move();
    }

    public interface IParentable<TParentType>
    {
        TParentType parent { get; set; }
    }

    public interface ICastable<TParentType> : IParentable<TParentType>
    {
        float currentLifetime { get; }
        float maxLifetime { get; set; }
    }

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

    public interface IStats : IAttackable
    {
        float maxMana { get; }
        // Mana(currency) property
        float mana { get; set; }

        // Experience property
        float experience { get; set; }
        // Level property
        int level { get; }
    }

    [Serializable]
    public struct SkillData
    {
        public GameObject skillPrefab;
        public float cooldown;
        public float remainingCooldown;

        public float cost;

        public Sprite sprite;

        public SkillData(GameObject a_SkillPrefab, float a_Cooldown, float a_RemainingCooldown, float a_Cost, Sprite a_Sprite) : this()
        {
            skillPrefab = a_SkillPrefab;
            cooldown = a_Cooldown;
            remainingCooldown = a_RemainingCooldown;

            cost = a_Cost;

            sprite = a_Sprite;
        }
    }

    public interface IUsesSkills : IStats
    {
        List<SkillData> skills { get; set; }
    }
}
