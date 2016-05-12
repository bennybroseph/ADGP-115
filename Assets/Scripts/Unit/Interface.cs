// Interface

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unit
{
    [Serializable]
    public struct Moving
    {
        public bool up;
        public bool down;
        public bool left;
        public bool right;

        public Moving(bool a_Forward, bool a_Back, bool a_Left, bool a_Right) : this()
        {
            up = a_Forward;
            down = a_Back;
            left = a_Left;
            right = a_Right;
        }

        public static Moving Nowhere = new Moving(false, false, false, false);
    }

    /// <summary>
    ///  Ensures the object can move using input or other stimulus 
    /// </summary>
    public interface IControlable
    {
        /// <summary> Which directions the object is moving in. </summary>
        Moving isMoving { get; set; }
        /// <summary> The speed at which the object is moving. </summary>
        Vector3 velocity { get; set; }

        bool canMoveWithInput { get; set; }

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

    public interface IAttackable
    {
        // String Name property
        string unitName { get; set; }

        // Health property
        int health { get; set; }
        // Defense property
        int defense { get; set; }
    }

    public interface IStats : IAttackable
    {
        // Mana(currency) property
        int mana { get; set; }

        // Speed property
        int speed { get; set; }

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
