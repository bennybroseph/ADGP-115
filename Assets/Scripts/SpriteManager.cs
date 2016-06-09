using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Library;
using Units.Skills;

public class SpriteManager : MonoSingleton<SpriteManager>
{
    [SerializeField]
    private List<Sprite> m_Sprites;

    public List<Sprite> Sprites
    {
        get { return m_Sprites;}
    }
    // Function for setting the appropriate sprite for upgrade
    public Sprite SetLevelUpSprite(Skill a_Skill)
    {
        Debug.Log(a_Skill.skillData.currentSprite);
        Sprite LeveledUpSprite = a_Skill.skillData.currentSprite;
        // Loop through m_Sprites list
        for (int index = 0; index < m_Sprites.Count - 1; index++)
        {// If the current sprite in the lists name is the same as the passed in sprites name
            if (m_Sprites[index].name == a_Skill.skillData.currentSprite.name)
            {// Check the current level of the skill
                switch (a_Skill.level)
                {
                    case 1:
                        // Set it to current index
                        LeveledUpSprite = m_Sprites[index];
                        break;
                    case 2:
                        // Set it to current index + 1
                        LeveledUpSprite = m_Sprites[index + 1];
                        break;
                    case 3:
                        // Set it to current index + 2
                        LeveledUpSprite = m_Sprites[index + 2];
                        break;
                }           
            }
        }
        // Return the sprite
        return LeveledUpSprite;
    }
}
