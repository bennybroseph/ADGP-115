﻿//////////////////////
//      Define      //
//////////////////////

namespace Define
{
    public enum Event
    {
        // Used for testing purposes
        Test,

        // There are no parameters
        NewGame,
        // There are no parameters
        LoadGame,
        // There are no parameters
        Instructions,
        // There are no parameters
        QuitGame,
        //There are no parameters
        MainMenu,

        // There are no parameters
        SpawnWaveClicked,

        /// <param name ="Wave Counter"> int: Wave counter</param> 
        SpawnWave,

        // There are no parameters
        ToggleQuitMenu,
        // There are no parameters
        GameOver,

        // There are no parameters
        PauseGame,
        // There are no paramters
        UnPauseGame,

        /// <param name="Unit"> IUsesSkills: which unit's skill should be upgraded per user input</param> 
        /// <param name="Skill Index"> int: which skill to upgrade</param>
        UpgradeSkill,
        /// <param name="Unit"> IUsesSkills: which unit's skill should be used per user input</param> 
        /// <param name="Skill Index"> int: which skill to use</param>
        UseSkill,
        
        /// <param name="Unit"> IStats: which unit's cooldown changed</param> 
        /// <param name="Skill Index"> int: which skill's cooldown changed</param>
        SkillCooldownChanged,

        /// <param name="Unit"> IStats: which unit was initialized</param>
        UnitInitialized,

        /// <param name="Unit"> IStats: which unit's health changed</param>
        UnitHealthChanged,
        /// <param name="Unit"> IStats: which unit's mana changed</param>
        UnitManaChanged,
        /// <param name="Unit"> IStats: which unit's level changed</param>
        UnitLevelChanged,
        /// <param name="Unit"> IStats: which unit leveled up</param>
        UnitLevelUp,

        /// <param name="Fortress"> IAttackable: which fortress was initialized</param>
        FortressInitialized,
        /// <param name="Fortress"> IAttackable: which fortress's health changed</param>
        FortressHealthChanged,
        /// <param name="Fortress"> IAttackable: which fortress died</param>
        FortressDied,

        /// <param name="Unit"> IStats: which unit died</param>
        UnitDied,
    }

}