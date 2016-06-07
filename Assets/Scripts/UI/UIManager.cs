using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

using Library;
using Units;
using Units.Controller;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Button = UnityEngine.UI.Button;
using Event = Define.Event;
using System;

namespace UI
{
    public class UIManager : MonoSingleton<UIManager>, IParentable
    {
        #region -- VARIABLES --
        [Header("Prefabs")]
        [SerializeField]
        private UIAnnouncer m_UIAnnouncerPrefab;
        [SerializeField]
        private Canvas m_BackgroundUIPrefab;
        [SerializeField]
        private UnitNameplate m_HUDPrefab;
        [SerializeField]
        private SkillButton m_SkillButtonPrefab;

        [Header("Other")]
        [SerializeField]
        private List<SkillButton> m_SkillButtons;
        [SerializeField]
        private Text m_WaveCounter;
        [SerializeField]
        private UnitNameplate m_HUD;
        [SerializeField]
        private RectTransform m_BattleUI;
        [SerializeField]
        private RectTransform m_QuitMenu;
        [SerializeField]
        private RectTransform m_InstructionMenu;
        [SerializeField]
        private RectTransform m_GameOverMenu;
        [SerializeField]
        private RectTransform m_OptionsMenu;
        [SerializeField]
        private Button m_NewGame;
        [SerializeField]
        private Button m_LoadGame;
        [SerializeField]
        private Button m_Instructions;
        [SerializeField]
        private Button m_Options;
        [SerializeField]
        private Button m_QuitGame;
        [SerializeField]
        private Canvas m_BackgroundUI;
        private Text m_AutoSpawnTimer;
        #endregion

        #region -- PROPERTIES --
        public Canvas backgroundUI
        {
            get { return m_BackgroundUI; }
        }
        public Text AutoSpawnTimer
        {
            get { return m_AutoSpawnTimer; }
            set { m_AutoSpawnTimer = value; }
        }

        #endregion

        #region -- UNITY FUNCTIONS --
        protected override void Awake()
        {
            base.Awake();

            m_SkillButtons = new List<SkillButton>();

            Instantiate(m_UIAnnouncerPrefab);
            m_BackgroundUI = Instantiate(m_BackgroundUIPrefab);

            GetComponents();

            Publisher.self.Subscribe(Event.Instructions, OnInstructions);
            Publisher.self.Subscribe(Event.Options, OnOptions);
            Publisher.self.Subscribe(Event.ToggleQuitMenu, OnToggleQuitMenu);
            Publisher.self.Subscribe(Event.SpawnWave, OnSpawnWave);
            Publisher.self.Subscribe(Event.MainMenu, OnMainMenu);
            Publisher.self.Subscribe(Event.GameOver, OnGameOver);
            Publisher.self.Subscribe(Event.GameWin, OnGameWin);
            Publisher.self.Subscribe(Event.ApplyClicked, OnApplyClicked);
            Publisher.self.Subscribe(Event.CancelClicked, OnCancelClicked);

            if (m_SkillButtonPrefab != null)
                Publisher.self.Subscribe(Event.UnitInitialized, OnUnitInitialized);
            else
                Debug.LogWarning("UIManager needs a 'Skill Button Prefab' in order to function properly");

        }

        // Use this for initialization
        private void Start()
        {
            if (m_HUDPrefab != null)
            {
                m_HUD = Instantiate(m_HUDPrefab);
                m_HUD.transform.SetParent(m_BackgroundUI.transform, false);
                m_HUD.transform.SetAsLastSibling();
                m_HUD.parent = FindObjectOfType<Player>().unit;
            }

            if (m_SkillButtonPrefab.GetComponent<RectTransform>() == null)
                return;

            List<IUsesSkills> skillUsers =
                FindObjectsOfType<GameObject>().
                    Where(
                        x => x.GetComponent<IControllable>() != null &&
                        x.GetComponent<IControllable>().controllerType == ControllerType.User &&
                        x.GetComponent<IUsesSkills>() != null).
                    Select(x => x.GetComponent<IUsesSkills>()).
                    ToList();

            skillUsers.Sort((a, b) => a.unitName.CompareTo(b.unitName));

            int numOfSkills = 0;
            foreach (IUsesSkills skillUser in skillUsers)
                numOfSkills += skillUser.skills.Count - 1;

            int k = 0;
            for (int i = 0; i < skillUsers.Count; i++)
            {
                for (int j = 0; j < skillUsers[i].skills.Count; j++)
                {
                    SkillButton skillButton = InstantiateRectTransform(
                        m_SkillButtonPrefab,
                        new Vector3(
                            k * 70 + i * 100 - (numOfSkills * 70 + (skillUsers.Count - 1) * 100) / 2,
                            0,
                            0));

                    skillButton.parent = skillUsers[i];
                    skillButton.skillIndex = j;
                    skillButton.sprite = skillUsers[i].skills[j].skillData.sprite;

                    m_SkillButtons.Add(skillButton);

                    k++;
                }
                k--;
            }
        }

        //LateUpdate is called once per frame
        private void LateUpdate()
        {

        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            Publisher.self.UnSubscribe(Event.Instructions, OnInstructions);
            Publisher.self.UnSubscribe(Event.Options, OnOptions);
            Publisher.self.UnSubscribe(Event.ToggleQuitMenu, OnToggleQuitMenu);
            Publisher.self.UnSubscribe(Event.SpawnWave, OnSpawnWave);
            Publisher.self.UnSubscribe(Event.MainMenu, OnMainMenu);
            Publisher.self.UnSubscribe(Event.GameOver, OnGameOver);
            Publisher.self.UnSubscribe(Event.GameWin, OnGameWin);

            if (m_SkillButtonPrefab != null)
                Publisher.self.UnSubscribe(Event.UnitInitialized, OnUnitInitialized);
        }
        #endregion

        #region -- PRIVATE VOID FUNCTIONS --
        private void GetComponents()
        {
            foreach (Transform child in transform)
            {
                switch (child.tag)
                {
                    case "Battle UI":
                        {
                            // Grab the child only if it wasn't set in the inspector
                            if (m_BattleUI == null)
                                m_BattleUI = child.GetComponent<RectTransform>();
                            // If component could not be found
                            if (m_BattleUI == null)
                            {
                                Debug.LogWarning("UIManager is missing an object with the 'Battle UI' tag parented to it");
                                continue;
                            }

                            Button spawnWaveButton = m_BattleUI.GetComponentsInChildren<Button>()[0];
                            Button instructionsButton = m_BattleUI.GetComponentsInChildren<Button>()[1];

                            m_AutoSpawnTimer = spawnWaveButton.GetComponentInChildren<Text>();

                            spawnWaveButton.onClick.AddListener(OnSpawnWaveClick);
                            instructionsButton.onClick.AddListener(OnInstructionsClick);
                            m_WaveCounter = m_BattleUI.GetComponentInChildren<Text>();
                        }
                        break;
                    case "Quit Menu":
                        {
                            if (m_QuitMenu == null)
                                m_QuitMenu = child.GetComponent<RectTransform>();
                            if (m_QuitMenu == null)
                            {
                                Debug.LogWarning("UIManager is missing an object with the 'Quit Menu' tag parented to it");
                                continue;
                            }

                            Button quitButton = m_QuitMenu.GetComponentsInChildren<Button>()[0];
                            Button resumeButton = m_QuitMenu.GetComponentsInChildren<Button>()[1];
                            Button optionButton = m_QuitMenu.GetComponentsInChildren<Button>()[2];

                            quitButton.onClick.AddListener(OnQuitGameClick);
                            resumeButton.onClick.AddListener(OnResumeClick);
                            optionButton.onClick.AddListener(OnOptionsClick);

                            m_QuitMenu.gameObject.SetActive(false);
                        }
                        break;
                    case "Instructions Menu":
                        {
                            if (m_InstructionMenu == null)
                                m_InstructionMenu = child.GetComponent<RectTransform>();
                            if (m_InstructionMenu == null)
                            {
                                //Displays a debug log warning detecting if the "Instructions Menu" tag is missing
                                Debug.LogWarning("UIManager is missing an object with the 'Instructions Menu' tag parented to it");
                                continue;
                            }

                            Button closeButton = m_InstructionMenu.GetComponentInChildren<Button>();

                            closeButton.onClick.AddListener(OnInstructionsCloseClick);

                            m_InstructionMenu.gameObject.SetActive(false);
                        }
                        break;
                    case "Options Menu":
                        {
                            if (m_OptionsMenu == null)
                                m_OptionsMenu = child.GetComponent<RectTransform>();
                            if (m_OptionsMenu == null)
                            {
                                //Displays a debug log warning detecting if the "Op" tag is missing
                                Debug.LogWarning("UIManager is missing an object with the 'Options Menu' tag parented to it");
                                continue;
                            }

                            Button applyButton = m_OptionsMenu.GetComponentsInChildren<Button>()[0];
                            Button cancelButton = m_OptionsMenu.GetComponentsInChildren<Button>()[1];
                            Button closeButton = m_OptionsMenu.GetComponentsInChildren<Button>()[2];
                            Slider volumeSlider = m_OptionsMenu.GetComponentsInChildren<Slider>()[0];
                            // Set volumeSlider value to a default value
                            volumeSlider.value = AudioManager.self.Sounds[0].Volume;

                            applyButton.onClick.AddListener(delegate { OnOptionsApplyClick(volumeSlider); });
                            cancelButton.onClick.AddListener(OnOptionsCancelClick);
                            closeButton.onClick.AddListener(OnOptionsCloseClick);
                            m_OptionsMenu.gameObject.SetActive(false);
                        }
                        break;
                    case "Game Over Menu":
                        {
                            if (m_GameOverMenu == null)
                                m_GameOverMenu = child.GetComponent<RectTransform>();
                            if (m_GameOverMenu == null)
                            {
                                //Displays a debug log warning detecting if the "Game Over Menu" tag is missing
                                Debug.LogWarning("UIManager is missing an object with the 'Game Over Menu' tag parented to it");
                                continue;
                            }

                            Button mainMenuButton = m_GameOverMenu.GetComponentsInChildren<Button>()[0];
                            Button quitButton = m_GameOverMenu.GetComponentsInChildren<Button>()[1];

                            mainMenuButton.onClick.AddListener(OnMainMenuClick);
                            quitButton.onClick.AddListener(OnQuitGameClick);

                            m_GameOverMenu.gameObject.SetActive(false);
                        }
                        break;
                    case "New Game":
                        {
                            if (m_NewGame == null)
                                m_NewGame = child.GetComponent<Button>();
                            if (m_NewGame == null)
                            {
                                //Displays a debug log warning detecting if the "New Game" tag is missing
                                Debug.LogWarning("UIManager is missing an object with the 'New Game' tag parented to it");
                                continue;
                            }

                            m_NewGame.onClick.AddListener(delegate { SceneManager.LoadScene("Andrew"); });
                        }
                        break;
                    case "Load Game":
                        {
                            if (m_LoadGame == null)
                                m_LoadGame = child.GetComponent<Button>();
                            if (m_LoadGame == null)
                            {
                                //Displays a debug log warning detecting if the "Load Game" tag is missing
                                Debug.LogWarning("UIManager is missing an object with the 'Load Game' tag parented to it");
                                continue;
                            }

                            m_LoadGame.onClick.AddListener(OnLoadGameClick);
                        }
                        break;
                    case "Instructions":
                        {
                            if (m_Instructions == null)
                                m_Instructions = child.GetComponent<Button>();
                            if (m_Instructions == null)
                            {
                                //Displays a debug log warning detecting if the "Instructions tag is missing
                                Debug.LogWarning("UIManager is missing an object with the 'Instructions' tag parented to it");
                                continue;

                            }

                            m_Instructions.onClick.AddListener(OnInstructionsClick);
                        }
                        break;
                    case "Options":
                        {
                            if (m_Options == null)
                                m_Options = child.GetComponent<Button>();
                            if (m_Options == null)
                            {
                                Debug.LogWarning("UIManger is missing an object with the 'Options' tag parented to it");
                            }

                            m_Options.onClick.AddListener(OnOptionsClick);

                        }
                        break;
                    case "Quit Game":
                        {
                            if (m_QuitGame == null)
                                m_QuitGame = child.GetComponent<Button>();
                            if (m_QuitGame == null)
                            {
                                //Displays a debug log warning detecting if the instructions tag is missing
                                Debug.LogWarning("UIManager is missing an object with the 'Quit Game' tag parented to it");
                                continue;
                            }

                            m_QuitGame.onClick.AddListener(delegate { Application.Quit(); });
                        }
                        break;
                }
            }
        }
        #endregion

        #region -- EVENT FUNCTIONS --

        private void OnUnitInitialized(Event a_Event, params object[] a_Params)
        {
                
        }

        private void OnMainMenu(Event a_Event, params object[] a_Params)
        {
            SceneManager.LoadScene(0);
        }

        private void OnToggleQuitMenu(Event a_Event, params object[] a_Params)
        {
            m_QuitMenu.gameObject.SetActive(!m_QuitMenu.gameObject.activeInHierarchy);
            Publisher.self.Broadcast(m_QuitMenu.gameObject.activeInHierarchy ? Event.PauseGame : Event.UnPauseGame);
        }

        private void OnInstructions(Event a_Event, params object[] a_Params)
        {
            m_InstructionMenu.gameObject.SetActive(true);
        }

        private void OnOptions(Event a_Event, params object[] a_Params)
        {
            m_OptionsMenu.gameObject.SetActive(true);
        }

        private void OnInstructionsClick()
        {
            Publisher.self.Broadcast(Event.Instructions);
        }

        private void OnOptionsClick()
        {
            Publisher.self.Broadcast(Event.Options);
        }

        private void OnInstructionsCloseClick()
        {
            m_InstructionMenu.gameObject.SetActive(false);
        }

        private void OnOptionsCloseClick()
        {
            m_OptionsMenu.gameObject.SetActive(false);
        }

        private void OnOptionsApplyClick(Slider a_VolumeSlider)
        {
            Publisher.self.Broadcast(Event.ApplyClicked, a_VolumeSlider);
        }

        private void OnApplyClicked(Event a_Event, params object[] a_Params)
        {
            Slider volumeSlider = a_Params[0] as Slider;

            foreach (Sound sound in AudioManager.self.Sounds)
            {
                sound.Source.volume = volumeSlider.value;
            }

            //Debug.Log("Apply Clicked!");
        }

        private void OnOptionsCancelClick()
        {
            Publisher.self.Broadcast(Event.CancelClicked);
        }

        private void OnCancelClicked(Event a_Event, params object[] a_Params)
        {
            Debug.Log("Cancel Clicked!");
        }

        private void OnResumeClick()
        {
            m_QuitMenu.gameObject.SetActive(false);
            m_OptionsMenu.gameObject.SetActive(false);
            Publisher.self.Broadcast(Event.UnPauseGame);
        }

        public void OnOptionClick()
        {
            m_QuitMenu.gameObject.SetActive(false);
        }

        private void OnSpawnWaveClick()
        {
            Publisher.self.Broadcast(Event.SpawnWaveClicked);
        }

        private void OnMainMenuClick()
        {
            Publisher.self.Broadcast(Event.UnPauseGame);
            Publisher.self.Broadcast(Event.MainMenu);

        }

        //Function for LoadGame button
        private void OnLoadGameClick()
        {
            //Load Game Function
            //Publisher Subscriber for LoadGame/ Broadcast
            Publisher.self.Broadcast(Event.LoadGame);
        }

        //Function for QuitGame button
        private void OnQuitGameClick()
        {
            //Publisher Subscriber or QuitGame / Broadcast 
            Publisher.self.Broadcast(Event.QuitGame);
        }

        private void OnSpawnWave(Event a_Event, params object[] a_Params)
        {
            int waveCounter = (int)a_Params[0];
            m_WaveCounter.text = " Wave: " + waveCounter;
        }
        #endregion

        #region -- PRIVATE FUNCTIONS --
        private SkillButton InstantiateRectTransform(SkillButton a_RectTransform, Vector3 a_Position)
        {
            SkillButton skillButton = Instantiate(a_RectTransform);
            skillButton.GetComponent<RectTransform>().SetParent(transform, false);
            skillButton.transform.SetAsFirstSibling();

            skillButton.transform.localPosition += a_Position;

            return skillButton;
        }

        private void OnGameOver(Event a_Event, params object[] a_Params)
        {
            m_GameOverMenu.gameObject.SetActive(true);

            Publisher.self.Broadcast(Event.PauseGame);
        }

        private void OnGameWin(Event a_Event, params object[] a_Params)
        {
            StartCoroutine(GameWinbroadcast());
        }

        private IEnumerator GameWinbroadcast()
        {
            AudioManager.self.PlaySound(SoundTypes.VictorySound);
            UIAnnouncer.self.Announce("You've Won!!");
            yield return new WaitForSeconds(5);
            Publisher.self.Broadcast(Event.GameOver);
        }
        #endregion
    }
}