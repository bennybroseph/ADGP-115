using UnityEngine;
using System.Collections.Generic;
using Units;
using Units.Skills;

namespace Interfaces
{
    public interface IStats : IAttackable, IParentable
    {
        float maxMana { get; }
        // Mana(currency) property
        float mana { get; set; }

        // Experience property
        float experience { get; set; }
        // Level property
        int level { get; }
    }

    public interface IUsesSkills : IStats
    {
        List<Skill> skills { get; }
    }
}
