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
    private IUsesSkills player;
    [SerializeField]
    private GameObject PlGameObject;

    // Use this for initialization
    void Start ()
    {
        PlGameObject = GameObject.FindGameObjectWithTag("Player");
        player = PlGameObject.GetComponent<IUsesSkills>();

    }
	
    public void OnPointerEnter(PointerEventData eventData)
    {
        string buttonname = gameObject.GetComponentInParent<SkillButton>().name;

        UIManager.self.toolTip.gameObject.SetActive(true);
        Text skillDataText = UIManager.self.toolTip.GetComponentInChildren<Text>();

        switch (buttonname)
        {
            case "Skill_Button_Prefab(Clone) 0":
                skillDataText.text = player.skills[0].skillData.description;
                break;

            case "Skill_Button_Prefab(Clone) 1":
                skillDataText.text = player.skills[1].skillData.description;
                break;

            case "Skill_Button_Prefab(Clone) 2":
                skillDataText.text = player.skills[2].skillData.description;
                break;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.self.toolTip.gameObject.SetActive(false);
    }
}
