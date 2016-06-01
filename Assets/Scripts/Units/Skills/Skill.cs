using System;
using Interfaces;
using UnityEngine;

using Library;

using Event = Define.Event;

namespace Units.Skills
{

    public enum BuffType
    {
        Buff,
        DeBuff,
        None
    }

    public enum DamageType
    {
        Physical,
        Magical,
        None
    }

    [Serializable]
    public class SkillData
    {
        public string name;
        public string description;

        public Sprite sprite;

        public int skillIndex;
        public float maxCooldown;

        public float damage;
        public float cost;

        public BuffType buffType;
        public DamageType damageType;

        public SkillData Clone()
        {
            SkillData clone = new SkillData();

            clone.name = name;
            clone.description = description;

            clone.sprite = sprite;

            clone.maxCooldown = maxCooldown;

            clone.damage = damage;
            clone.cost = cost;

            clone.buffType = buffType;
            clone.damageType = damageType;

            return clone;
        }
        //public SkillData(float a_MaxCooldown, float a_Damage, float a_Cost)
        //{
        //    maxCooldown = a_MaxCooldown;

        //    damage = a_Damage;
        //    cost = a_Cost;
        //}
    }

    [Serializable]
    public class Skill : IChildable<IUsesSkills>
    {
        #region -- VARIABLES --
        [SerializeField]
        private IUsesSkills m_Parent;

        [SerializeField]
        private int m_Level;

        [SerializeField]
        private GameObject m_SkillPrefab;

        [SerializeField]
        private SkillData m_SkillData;
        [SerializeField, ReadOnly]
        private int m_SkillIndex;

        [SerializeField, Range(0.0f, 1.0f)]
        private float m_CooldownReduction;
        [SerializeField]
        private float m_RemainingCooldown;
        #endregion

        #region -- PROPERTIES --
        public IUsesSkills parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        public int level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public GameObject skillPrefab
        {
            get { return m_SkillPrefab; }
            set { m_SkillPrefab = value; }
        }

        public SkillData skillData
        {
            get { return m_SkillData; }
            set { m_SkillData = value; }
        }
        public int skillIndex
        {
            get { return m_SkillIndex; }
            set { m_SkillIndex = value; }
        }
        public float cooldownReduction
        {
            get { return m_CooldownReduction; }
            set { m_CooldownReduction = value; }
        }
        public float remainingCooldown
        {
            get { return m_RemainingCooldown; }
            private set { m_RemainingCooldown = value; }
        }
        #endregion

        #region -- PUBLIC FUNCTIONS --
        public void ChangeCoolDown(float a_RemainingCooldown)
        {
            m_RemainingCooldown = a_RemainingCooldown;

            if (remainingCooldown < 0.0f)
                remainingCooldown = 0.0f;
        }

        public void UpdateCooldown()
        {
            if (m_RemainingCooldown == 0.0f)
                return;

            ChangeCoolDown(m_RemainingCooldown - Time.deltaTime);

            Publisher.self.Broadcast(Event.SkillCooldownChanged, m_Parent, m_SkillIndex);
        }

        public void PutOnCooldown()
        {
            ChangeCoolDown(skillData.maxCooldown * (1.0f - cooldownReduction));
        }

        #endregion
    }
}