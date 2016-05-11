// Interface

using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Unit
{
    [Serializable]
    public struct Moving
    {
        public bool forward { get; set; }
        public bool back { get; set; }
        public bool left { get; set; }
        public bool right { get; set; }

        public Moving(bool a_Forward, bool a_Back, bool a_Left, bool a_Right) : this()
        {
            forward = a_Forward;
            back = a_Back;
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

    public interface IAttackable
    {
        // Function for combat
        void Fight();
    }

    public interface IStats
    {
        // Health property
        int health { get; set; }
        // Mana(currency) property
        int mana { get; set; }
        // Speed property
        int speed { get; set; }
        // Defense property
        int defense { get; set; }
        // Experience property
        int experience { get; set; }
        // Level property
        int level { get; set; }
        // String Name property
        string unitName { get; set; }
    }
}
