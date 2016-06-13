using UnityEngine;
using Units.Skills;

namespace Interfaces
{
    public interface IParentable
    {
        GameObject gameObject { get; }
    }

    public interface IChildable<TParentType> where TParentType : IParentable
    {
        TParentType parent { get; set; }
    }

    public interface ICastable<TParentType> : IChildable<TParentType> where TParentType : IParentable
    {
        float baseMaxCooldown { get; }
        float maxCooldownGrowth { get; }

        float baseDamage { get; }
        float damageGrowth { get; }

        float baseCost { get; }
        float costGrowth { get;}

        float currentLifetime { get; }
        float maxLifetime { get; set; }

        SkillData skillData { get; set; }

        string UpdateDescription(SkillData a_SkillData);
    }
}
