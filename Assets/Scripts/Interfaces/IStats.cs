using System.Collections.Generic;
using Units.Skills;

namespace Interfaces
{
    #region -- PUBLIC INTERFACES
    public interface IStats : IAttackable, IParentable
    {
        float maxMana { get; }
        // Mana(currency) property
        float mana { get; set; }

        // Experience property
        float experience { get; set; }
        // Level property
        int level { get; }

        void GetLevelBar(out float a_LastLevel, out float a_NextLevel);
    }

    public interface IUsesSkills : IStats
    {
        List<Skill> skills { get; }
    }
#endregion
}
