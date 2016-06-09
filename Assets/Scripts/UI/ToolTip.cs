using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Define;
using Interfaces;
using UI;
using Units.Skills;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private IUsesSkills m_Player;
    // Use this for initialization
    void Start ()
    {// Grab the player object in the scene
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<IUsesSkills>();
    }
	// When the mouse enters the gameobject this object is attached to and its an event trigger
    public void OnPointerEnter(PointerEventData a_EventData)
    {// Grab the proper components
        int skillindex = gameObject.GetComponentInParent<SkillButton>().skillIndex;
        Text skillDataText = UIManager.self.toolTip.GetComponentInChildren<Text>();
        // Activate the tooltip menu
        UIManager.self.toolTip.gameObject.SetActive(true);

        if (gameObject.name == "Upgrade Button")
        {
            // Update the text with the appropriate skill description
            BaseSkill skill = m_Player.baseSkills[skillindex].GetComponent<BaseSkill>();
            SkillData skillData = skill.GetSkillData(m_Player.skills[skillindex].level + 1);
            skillDataText.text =
                skillData.name + " - Cost: " + skillData.cost + "\n" +
                "-------------------------------\n" +
                skillData.description;
        }
        else
        {
            // Update the text with the appropriate skill description
            BaseSkill skill = m_Player.baseSkills[skillindex].GetComponent<BaseSkill>();
            SkillData skillData = skill.GetSkillData(m_Player.skills[skillindex].level);
            skillDataText.text =
                skillData.name + " - Cost: " + skillData.cost + "\n" +
                "-------------------------------\n" +
                skillData.description;
        }
    }
    // When the mouse exits the gameobject this object is attached to and its an event trigger
    public void OnPointerExit(PointerEventData a_EventData)
    {// Deactivate the tooltip menu
        UIManager.self.toolTip.gameObject.SetActive(false);
    }
}
