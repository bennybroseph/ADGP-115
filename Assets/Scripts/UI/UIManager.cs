using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Library;
using Define;

using Event = Define.Event;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_QuitMenu;
        [SerializeField]
        private GameObject m_InstructionMenu;

        [SerializeField]
        private List<GameObject> m_SkillButtons;

        [SerializeField]
        private GameObject m_Skill1Upgrade;
        [SerializeField]
        private GameObject m_Skill2Upgrade;
        [SerializeField]
        private GameObject m_Player;
        [SerializeField]
        private GameObject m_PlayerUI;
        [SerializeField]
        private Vector3 m_PlayerUIOffset;

        // Use this for initialization
        void Start()
        {
            if (m_InstructionMenu != null)
                m_InstructionMenu.SetActive(false);

            if (m_Skill1Upgrade != null)
                m_Skill1Upgrade.SetActive(false);
            if (m_Skill2Upgrade != null)
                m_Skill2Upgrade.SetActive(false);

            if (m_QuitMenu != null)
                m_QuitMenu.SetActive(false);

            if (m_Player == null)
                m_Player = GameObject.FindGameObjectWithTag("Player");

            Publisher.self.Subscribe(Event.Instructions, OnInstructions);

            Publisher.self.Subscribe(Event.SkillCooldownChanged, OnSkillCooldownChanged);
        }

        //LateUpdate is called once per frame
        void LateUpdate()
        {
            if (m_PlayerUI != null && m_Player != null)
                m_PlayerUI.transform.position = m_Player.transform.position + m_PlayerUIOffset;
        }

        private void OnInstructions(Event a_Event, params object[] a_Params)
        {
            // Do stuff...
            m_InstructionMenu.SetActive(true);

        }

        public void OnInstructionsClick()
        {
            Publisher.self.Broadcast(Event.Instructions);
        }

        public void OnInstructionsCloseClick()
        {
            m_InstructionMenu.SetActive(false);
        }

        public void OnResumeClick()
        {
            m_QuitMenu.SetActive(false);
            Publisher.self.Broadcast(Event.UnPauseGame);
        }

        public void OnQuitGameClick()
        {
            Publisher.self.Broadcast(Event.QuitGame);
        }

        public void OnUpgradeSkill1()
        {
            Publisher.self.Broadcast(Event.UpgradeSkill, 1);
        }

        public void OnUpgradeSkill2()
        {
            Publisher.self.Broadcast(Event.UpgradeSkill, 2);
        }

        public void OnUseSkill1()
        {
            Publisher.self.Broadcast(Event.UseSkill, 1);
        }

        public void OnUseSkill2()
        {
            Publisher.self.Broadcast(Event.UseSkill, 2);
        }

        public void OnSpawnWaveClick()
        {
            Publisher.self.Broadcast(Event.SpawnWave);
        }

        //Function for NewGame button
        public void NewGame()
        {
            //Publisher Subscriber for NewGame / Broadcast
            Publisher.self.Broadcast(Event.NewGame);

        }
        //Function for LoadGame button
        public void LoadGame()
        {
            //Load Game Function
            //Publisher Subscriber for LoadGame/ Broadcast
            Publisher.self.Broadcast(Event.LoadGame);
        }
        //Function for Instructions
        public void Instructions()
        {
            //Publisher Subscriber for Instructions / Broadcast 
            Publisher.self.Broadcast(Event.Instructions);
        }
        //Function for QuitGame button
        public void QuitGame()
        {
            //Publisher Subscriber or QuitGame / Broadcast 
            Publisher.self.Broadcast(Event.QuitGame);
        }

        private void OnSkillCooldownChanged(Event a_Event, params object[] a_Params)
        {
            int skillIndex = (int)a_Params[0];
            float remainingCooldown = (float)a_Params[1];

            string parsedCooldown;
            if (remainingCooldown == 0.0f)
            {
                parsedCooldown = "";
                m_SkillButtons[skillIndex].GetComponent<Image>().color = Color.white;
            }
            else
            {
                parsedCooldown = ((int) Mathf.Ceil(remainingCooldown)).ToString();
                m_SkillButtons[skillIndex].GetComponent<Image>().color = Color.gray;
            }

            m_SkillButtons[skillIndex].GetComponentInChildren<Text>().text = parsedCooldown;
        }
    }
}