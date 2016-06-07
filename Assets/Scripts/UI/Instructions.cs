using System;
using System.Collections.Generic;
using Interfaces;
using Units;
using Units.Controller;
using Units.Skills;
using UnityEngine;
using UnityEngine.UI;

public class Instructions : MonoBehaviour
{
    [SerializeField]
    private Text m_InstructionsText;

    // Use this for initialization
    void Start()
    {
        Player player = FindObjectOfType<Player>();

        List<Skill> skills = new List<Skill>();
        if (player != null)
            skills = FindObjectOfType<Player>().unit.skills;

        m_InstructionsText.text +=
            "Instructions:" + "\n" +

            "Keyboard Controls:" + "\n";

        int i = 0;
        //A foreach loop that goes through each skill on the instructions list and assigns them there description.
        foreach (Key<KeyCode> skillKey in KeyConfiguration.self.userConfigurations[0].skillKeys)
        {
            m_InstructionsText.text +=
                Enum.GetName(typeof(KeyCode), skillKey.keyCode) +
                " - Use ";

            //Checks to see if skill count = 0
            if (skills.Count == 0)
            {   //if skill count = 0 then set skill as its index.
                m_InstructionsText.text += "Skill " + (i + 1);
            }
            else
            {
                m_InstructionsText.text += skills[i].skillData.name;
            }

            m_InstructionsText.text += "\n";
            ++i;
        }

        m_InstructionsText.text +=
                Enum.GetName(typeof(KeyCode), KeyConfiguration.self.userConfigurations[0].verticalKeyAxis.positive.keyCode) +
                " - Moves Character Up " + "\n";
        m_InstructionsText.text +=
        Enum.GetName(typeof(KeyCode), KeyConfiguration.self.userConfigurations[0].verticalKeyAxis.negative.keyCode) +
               " - Move Character Down" + "\n";
        m_InstructionsText.text +=
             Enum.GetName(typeof(KeyCode), KeyConfiguration.self.userConfigurations[0].horizontalKeyAxis.positive.keyCode) +
                " - Moves Character Right " + "\n";
        m_InstructionsText.text +=
        Enum.GetName(typeof(KeyCode), KeyConfiguration.self.userConfigurations[0].horizontalKeyAxis.negative.keyCode) +
               " - Move Character Left " + "\n";
        m_InstructionsText.text +=
            Enum.GetName(typeof(KeyCode), KeyConfiguration.self.userConfigurations[0].targetModeKey.keyCode) +
            "   - Targets Enemy Unit " + "\n";

        m_InstructionsText.text +=
            Enum.GetName(typeof(KeyCode), KeyConfiguration.self.userConfigurations[0].switchTargetKey.keyCode) +
            " - Switches Targets " + "\n";



        m_InstructionsText.text +=

        "\n" + "Controller Controls:" + "\n";
        //sets index to 0
        i = 0;
        foreach (Key<ButtonCode> skillButton in KeyConfiguration.self.userConfigurations[0].skillButtons)
        {
            m_InstructionsText.text +=
                Enum.GetName(typeof(ButtonCode), skillButton.keyCode) +
                " - Use ";
            if (skills.Count == 0)
                m_InstructionsText.text += "Skill " + (i + 1);
            else
                m_InstructionsText.text += skills[i].skillData.name;


            m_InstructionsText.text += "\n";
            ++i;
        }

        m_InstructionsText.text +=
         Enum.GetName(typeof(ButtonCode), KeyConfiguration.self.userConfigurations[0].verticalButtonAxis.positive.keyCode) +
                 " - Move Character Up " + "\n";
        m_InstructionsText.text +=
       Enum.GetName(typeof(ButtonCode), KeyConfiguration.self.userConfigurations[0].verticalButtonAxis.negative.keyCode) +
               " - Move Character Down " + "\n";
        m_InstructionsText.text +=
       Enum.GetName(typeof(ButtonCode), KeyConfiguration.self.userConfigurations[0].horizontalButtonAxis.positive.keyCode) +
               " - Move Character Right " + "\n";
        m_InstructionsText.text +=
       Enum.GetName(typeof(ButtonCode), KeyConfiguration.self.userConfigurations[0].horizontalButtonAxis.negative.keyCode) +
               " - Move Character Left " + "\n";

        m_InstructionsText.text +=
            Enum.GetName(typeof(ButtonCode), KeyConfiguration.self.userConfigurations[0].targetModeButton.keyCode) +
            " - Targets Enemy Unit " + "\n";



        m_InstructionsText.text +=


         "\n" + "Upgrade:" + "\n" +
                "\t" + "'+' - Allows upgrade of specific skill" + "\n" +
                "\t" + "(Only available when able to level up)";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
