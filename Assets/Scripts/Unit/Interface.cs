// Interface

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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

        public static Moving Nowhere = new Moving(false, false, false, false);

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

    public interface IParentable
    {
        GameObject parent { get; set; }
    }

    public interface ICastable : IParentable
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
        string unitName { get; set; }

        // Health property
        int health { get; set; }
        // Defense property
        int defense { get; set; }

        FiniteStateMachine<DamageState> damageFSM { get; }
    }

    public interface IStats : IAttackable
    {
        // Mana(currency) property
        int mana { get; set; }

        // Experience property
        int experience { get; set; }
        // Level property
        int level { get; set; }
    }

    [Serializable]
    public struct SkillData
    {
        public GameObject skillPrefab;
        public float cooldown;
        public float remainingCooldown;

        public SkillData(GameObject a_SkillPrefab, float a_Cooldown, float a_RemainingCooldown = 0.0f) : this()
        {
            skillPrefab = a_SkillPrefab;
            cooldown = a_Cooldown;
            remainingCooldown = a_RemainingCooldown;
        }
    }

    public interface IUsesSkills : IStats
    {
        List<SkillData> skills { get; set; }
    }
}
