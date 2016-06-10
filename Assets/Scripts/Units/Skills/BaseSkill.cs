using System.Collections;
using System.Net;
using UnityEngine;
using Interfaces;

namespace Units.Skills
{
    public abstract class BaseSkill : MonoBehaviour, ICastable<IUsesSkills>
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

        public abstract string UpdateDescription(SkillData a_SkillData);

        public virtual SkillData GetSkillData(int a_Level)
        {
            a_Level--;
            if (a_Level < 0)
                a_Level = 0;

            SkillData newSkillData = new SkillData();
            newSkillData.name = m_SkillData.name;
            newSkillData.currentSprite = m_SkillData.sprites[a_Level <= 2 ? a_Level : 2];
            newSkillData.damage = m_BaseDamage + m_DamageGrowth * a_Level;
            newSkillData.cost = m_BaseCost + m_CostGrowth * a_Level;
            newSkillData.maxCooldown = m_BaseMaxCooldown + m_MaxCooldownGrowth * a_Level;
            newSkillData.description = UpdateDescription(newSkillData);
            return newSkillData;
        }

        protected virtual void Awake()
        {
            StartCoroutine(DestroyMe());
        }

        protected IEnumerator DestroyMe()
        {
            yield return new WaitForSeconds(m_MaxLifetime);

            Destroy(gameObject);
        }
    }
}