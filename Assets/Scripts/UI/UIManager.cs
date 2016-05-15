using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Library;
using Unit;

using Button = UnityEngine.UI.Button;
using Event = Define.Event;

namespace UI
{
    public class UIManager : MonoSingleton<UIManager>, IParentable
    {
        #region -- VARIABLES --
        [SerializeField]
        private SkillButton m_SkillButtonPrefab;

        [SerializeField]
        private RectTransform m_HUD;
        [SerializeField]
        private RectTransform m_QuitMenu;
        [SerializeField]
        private RectTransform m_InstructionMenu;
        #endregion

        #region -- UNITY FUNCTIONS --
        private void Awake()
        {
            List<Player> players = new List<Player>();

            foreach (Player player in FindObjectsOfType<Player>())
            {
                if (player.parent != null) continue;

                player.parent = this;
                players.Add(player);
            }

            if (m_SkillButtonPrefab.GetComponent<RectTransform>() == null)
                return;

            players.Sort(
                (a, b) => a.unitName.CompareTo(b.unitName));

            int numOfSkills = 0;
            foreach (Player player in players) { numOfSkills += player.skills.Count - 1; }
            if (numOfSkills == 0) return;

            int k = 0;
            for (int i = 0; i < players.Count; i++)
            {
                for (int j = 0; j < players[i].skills.Count; j++)
                {
                    SkillButton skillButton = InstantiateRectTransform(
                        m_SkillButtonPrefab,
                        new Vector3(
                            k * 70 + i * 100 - (numOfSkills * 70 + (players.Count - 1) * 100) / 2,
                            0,
                            0));

                    skillButton.parent = players[i];
                    skillButton.skillIndex = j;
                    skillButton.sprite = players[i].skills[j].sprite;

                    k++;
                }
                k--;
            }

            GetComponents();

            Publisher.self.Subscribe(Event.Instructions, OnInstructions);
            Publisher.self.Subscribe(Event.ToggleQuitMenu, OnToggleQuitMenu);
        }

        // Use this for initialization
        private void Start()
        {

        }

        //LateUpdate is called once per frame
        private void LateUpdate()
        {

        }
        #endregion

        private void GetComponents()
        {
            foreach (Transform child in transform)
            {
                switch (child.tag)
                {
                    case "HUD":
                        {
                            if (m_HUD == null)
                                m_HUD = child.GetComponent<RectTransform>();

                            Button spawnWaveButton = m_HUD.GetComponentsInChildren<Button>()[0];
                            Button instructionsButton = m_HUD.GetComponentsInChildren<Button>()[1];

                            spawnWaveButton.onClick.AddListener(OnSpawnWaveClick);
                            instructionsButton.onClick.AddListener(OnInstructionsClick);
                        }
                        break;
                    case "Quit Menu":
                        {
                            if (m_QuitMenu == null)
                                m_QuitMenu = child.GetComponent<RectTransform>();

                            Button quitButton = m_QuitMenu.GetComponentsInChildren<Button>()[0];
                            Button resumeButton = m_QuitMenu.GetComponentsInChildren<Button>()[1];

                            quitButton.onClick.AddListener(OnQuitGameClick);
                            resumeButton.onClick.AddListener(OnResumeClick);

                            m_QuitMenu.gameObject.SetActive(false);
                        }
                        break;
                    case "Instructions Menu":
                        {
                            if (m_InstructionMenu == null)
                                m_InstructionMenu = child.GetComponent<RectTransform>();

                            Button closeButton = m_InstructionMenu.GetComponentInChildren<Button>();

                            closeButton.onClick.AddListener(OnInstructionsCloseClick);

                            m_InstructionMenu.gameObject.SetActive(false);
                        }
                        break;
                }
            }
        }

        #region -- EVENT FUNCTIONS --

        private void OnToggleQuitMenu(Event a_Event, params object[] a_Params)
        {
            m_QuitMenu.gameObject.SetActive(!m_QuitMenu.gameObject.activeInHierarchy);
            Publisher.self.Broadcast(m_QuitMenu.gameObject.activeInHierarchy ? Event.PauseGame : Event.UnPauseGame);
        }

        private void OnInstructions(Event a_Event, params object[] a_Params)
        {
            // Do stuff...
            m_InstructionMenu.gameObject.SetActive(true);
        }

        public void OnInstructionsClick()
        {
            Publisher.self.Broadcast(Event.Instructions);
        }

        public void OnInstructionsCloseClick()
        {
            m_InstructionMenu.gameObject.SetActive(false);
        }

        public void OnResumeClick()
        {
            m_QuitMenu.gameObject.SetActive(false);
            Publisher.self.Broadcast(Event.UnPauseGame);
        }

        public void OnQuitGameClick()
        {
            Publisher.self.Broadcast(Event.QuitGame);
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
        #endregion

        private SkillButton InstantiateRectTransform(SkillButton a_RectTransform, Vector3 a_Position)
        {
            SkillButton skillButton = Instantiate(a_RectTransform);
            skillButton.GetComponent<RectTransform>().SetParent(transform, false);

            skillButton.transform.localPosition += a_Position;

            return skillButton;
        }

    }
}