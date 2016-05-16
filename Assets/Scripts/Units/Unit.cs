using UnityEngine;
using System.Collections.Generic;
using Library;
using Units.Skills;

namespace Units
{
    public class Unit : MonoBehaviour, IUsesSkills, IControlable
    {
        private IController m_Controller;
        private ControllerType m_ControllerType;
        private FiniteStateMachine<DamageState> m_DamageFSM;
        private FiniteStateMachine<MovementState> m_MovementFSM;
        private List<Skill> m_Skills;
        private string m_UnitName;
        private string m_UnitNickname;
        [SerializeField]
        private float m_MaxMana;
        private float m_Mana;
        [SerializeField]
        private float m_MaxDefense;
        private float m_Defense;
        [SerializeField]
        private float m_MaxHealth;
        private float m_Health;

        private float m_Experience;
        private int m_Level;

        private Vector3 m_TotalVelocity;
        private Vector3 m_Velocity;



        private float m_Speed;
        private Moving m_IsMoving;

        private bool m_CanMoveWithInput;



        public IController controller
        {
            get { return m_Controller; }
            set { m_Controller = value; }
        }

        public ControllerType controllerType
        {
            get { return m_ControllerType; }
            set { m_ControllerType = value; }
        }

        public FiniteStateMachine<DamageState> damageFSM
        {
            get { return m_DamageFSM; }
            set { m_DamageFSM = value; }
        }

        public FiniteStateMachine<MovementState> movementFSM
        {
            get { return m_MovementFSM; }
            set { m_MovementFSM = value; }
        }

        public List<Skill> skills
        {
            get { return m_Skills; }
            set { m_Skills = value; }
        }

        //string name property
        public string unitName
        {
            get { return m_UnitName; }
            set { m_UnitName = value; }
        }
        //string nickname property
        public string unitNickname
        {
            get { return m_UnitNickname; }
            set { m_UnitNickname = value; }
        }
        //max mana int property
        public float maxMana
        {
            get { return m_MaxMana; }
            set { m_MaxMana = value; }
        }

        //mana int property
        public float mana
        {
            get { return m_Mana; }
            set { m_Mana = value; }
        }

        //Max defense int property
        public float maxDefense
        {
            get { return m_MaxDefense; }
            set { m_MaxDefense = value; }
        }
        //Defense int property
        public float defense
        {
            get { return m_Defense; }
            set { m_Defense = value; }
        }

        //maxhealth int property
        public float maxHealth
        {
            get { return m_MaxHealth; }
            set { m_MaxHealth = value; }
        }

        //health int property
        public float health
        {
            get { return m_Health; }
            set { m_Health = value; }
        }
        //Experience int property
        public float experience
        {
            get { return m_Experience; }
            set { m_Experience = value; }
        }

        //Level int property
        public int level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        //totalVelecotiy Vector3 property 
        public Vector3 totalVelocity
        {
            get { return m_TotalVelocity; }
            set { m_TotalVelocity = value; }
        }
        //Vector
        public Vector3 velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }


        //Speed int property
        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }

        public Moving isMoving
        {
            get { return m_IsMoving; }
            set { m_IsMoving = value; }
        }

        //canMoveWithInput bool property
        public bool canMoveWithInput
        {
            get { return m_CanMoveWithInput; }
            set { m_CanMoveWithInput = value; }
        }


       


        



    }
}
