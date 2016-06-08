using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UI;
using Units.Skills;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private IUsesSkills m_Player;
    [SerializeField]
    private Sprite m_SkillImage;
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
        m_SkillImage = gameObject.GetComponentInParent<Image>().sprite;
        // Activate the tooltip menu
        UIManager.self.toolTip.gameObject.SetActive(true);
        // Update the text with the appropriate skill description
        skillDataText.text = m_Player.skills[skillindex].skillData.description;
        // Set text color to white to make it readable
        skillDataText.color = Color.white;
        // Set the tooltip image to match the image of the button respectively
        UIManager.self.toolTip.GetComponent<Image>().sprite = m_SkillImage;
    }
    // When the mouse exits the gameobject this object is attached to and its an event trigger
    public void OnPointerExit(PointerEventData a_EventData)
    {// Deactivate the tooltip menu
        UIManager.self.toolTip.gameObject.SetActive(false);
    }
}
