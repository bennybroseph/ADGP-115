using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
            Publisher.self.Subscribe(Event.Instructions, OnInstructions);

            if (m_InstructionMenu != null)
                m_InstructionMenu.SetActive(false);
            if (m_Skill1Upgrade != null)
                m_Skill1Upgrade.SetActive(false);
            if (m_Skill2Upgrade != null)
                m_Skill2Upgrade.SetActive(false);
            if (m_QuitMenu != null)
                m_QuitMenu.SetActive(false);


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
    }
}