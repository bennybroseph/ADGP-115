using System;
using Units.Controller;
using UnityEngine;
using UnityEngine.UI;

public class Instructions : MonoBehaviour
{
    [SerializeField]
    private Text m_InstructionsText;

    // Use this for initialization
    void Start()
    {
        m_InstructionsText.text =
            "Instructions:" + "\n" +

            "Keyboard Controls:" + "\n";
        foreach (Key<KeyCode> skillKey in KeyConfiguration.self.userConfigurations[0].skillKeys)
        {
            m_InstructionsText.text +=
                Enum.GetName(typeof(KeyCode), skillKey.keyCode) +
                " - Use Fireball" + "\n";
        }
        
       
        m_InstructionsText.text +=

        "Controller Controls:" + "\n";
       foreach (Key<ButtonCode> skillButton in KeyConfiguration.self.userConfigurations[0].skillButtons)
       {
            m_InstructionsText.text +=
                Enum.GetName(typeof(ButtonCode), skillButton.keyCode) +
                " Use Fireball" + "\n";
        }
        m_InstructionsText.text +=
          

            "Upgrade:" + "\n" +
                "\t" + "'+' - Allows upgrade of specific skill" + "\n" +
                "\t" + "(Only available when able to level up)";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
