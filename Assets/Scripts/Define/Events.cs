//////////////////////
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
        // There are no parameters
        PauseGame,
        // There are no paramters
        UnPauseGame,
        /// <param name="Skill Index"> int: which skill to upgrade</param>
        UpgradeSkill,
        /// <param name="Skill Index"> int: which skill to use</param>
        UseSkill,
        // There are no parameters
        SpawnWave,
        // There are no parameters
        GameOver,
        /// <param name="Unit"> IStats: which player's cooldown changed</param> 
        /// <param name="Skill Index"> int: which skill's cooldown changed</param>
        SkillCooldownChanged,

        UnitInitialized,
        /// <param name="Unit"> IStats: which unit's health changed</param>
        UnitHealthChanged,
        /// <param name="Unit"> IStats: which unit's mana changed</param>
        UnitManaChanged,
        /// <param name="Unit"> IStats: which unit's level changed</param>
        UnitLevelChanged
    }

}