using UnityEngine;
using System.Collections;
using Interfaces;
using Library;
using UI;
using Event = Define.Event;

namespace Units.Skills
{
    public abstract class BaseSkills : MonoBehaviour, ICastable<IUsesSkills>
    {
        #region -- VARIABLES --
        [SerializeField]
        protected SkillData m_SkillData;

        [SerializeField]
        protected float m_CurrentLifetime;
        [SerializeField]
        protected float m_MaxLifetime;
        [SerializeField]
        protected float m_BaseMaxCooldown;
        [SerializeField]
        protected float m_MaxCooldownGrowth;
        [SerializeField]
        protected float m_BaseDamage;
        [SerializeField]
        protected float m_DamageGrowth;
        [SerializeField]
        protected float m_BaseCost;
        [SerializeField]
        protected float m_CostGrowth;

        [SerializeField]
        protected Moving m_IsMoving;

        [SerializeField]
        protected IUsesSkills m_Parent;
        #endregion

        #region -- PROPERTIES --
        public SkillData skillData
        {
            get { return m_SkillData; }
            set { m_SkillData = value; }
        }

        public float baseMaxCooldown
        {
            get { return m_BaseMaxCooldown; }
        }

        public float maxCooldownGrowth
        {
            get { return m_MaxCooldownGrowth; }
        }

        public float baseDamage
        {
            get { return m_BaseDamage; }
        }

        public float damageGrowth
        {
            get { return m_DamageGrowth; }
        }

        public float baseCost
        {
            get { return m_BaseCost; }
        }

        public float costGrowth
        {
            get { return m_CostGrowth; }
        }

        public float currentLifetime
        {
            get { return m_CurrentLifetime; }
        }
        public float maxLifetime
        {
            get { return m_MaxLifetime; }
            set { m_MaxLifetime = value; }
        }

        public Moving isMoving
        {
            get { return m_IsMoving; }
            set { m_IsMoving = value; }
        }
        public bool canMoveWithInput { get; set; }

        public IUsesSkills parent
        {
            get { return m_Parent; }
            set { m_Parent = value;
            }
        }
        #endregion

        public abstract string UpdateDescription(Skill a_Skill);

    }
}