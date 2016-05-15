using UnityEngine;
using System.Collections.Generic;
#if !UNITY_WEBGL
using XInputDotNetPure; // Required in C#
#endif
using UnityEngine.SceneManagement;

using Library;
using UI;
using Unit;
using Event = Define.Event;


public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private UIManager m_UIManager;
    [SerializeField]
    private SpawnWaves m_WaveSpawner;
    [SerializeField]
    private float m_PreviousTimeScale;

    private GameObject m_Background;

#if !UNITY_WEBGL
    private bool m_PlayerIndexSet = false;
    [SerializeField]
    private PlayerIndex m_PlayerIndex;
    private GamePadState m_State;
    private GamePadState m_PrevState;
#endif

#if !UNITY_WEBGL
    public PlayerIndex playerIndex
    {
        get { return m_PlayerIndex; }
    }
    public GamePadState state
    {
        get { return m_State; }
    }
    public GamePadState prevState
    {
        get { return m_PrevState; }
    }
#endif

    private void Awake()
    {
        Instantiate(m_WaveSpawner);
        Instantiate(m_UIManager);
    }
    // Use this for initialization
    private void Start()
    {

        m_PreviousTimeScale = Time.timeScale;

        Application.targetFrameRate = -1;

        Publisher.self.Subscribe(Event.NewGame, OnNewGame);
        Publisher.self.Subscribe(Event.QuitGame, OnQuitGame);

        Publisher.self.Subscribe(Event.PauseGame, OnPauseGame);
        Publisher.self.Subscribe(Event.UnPauseGame, OnUnPauseGame);


    }

    // Update is called once per frame
    private void Update()
    {
        Publisher.self.Update();

#if !UNITY_WEBGL
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        if (!m_PlayerIndexSet || !m_PrevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    //Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    m_PlayerIndex = testPlayerIndex;
                    m_PlayerIndexSet = true;
                }
            }
        }

        m_PrevState = m_State;
        m_State = GamePad.GetState(m_PlayerIndex);
#endif

        if (Input.GetKeyDown(KeyCode.P))
            Publisher.self.Broadcast(Event.ToggleQuitMenu);
    }

    private void OnNewGame(Event a_Event, params object[] a_Params)
    {
        //Loads the first scene.
        SceneManager.LoadScene(1);
    }

    private void OnQuitGame(Event a_Event, params object[] a_Params)
    {
        //Used to Quit Application
        Application.Quit();
    }

    private void OnPauseGame(Event a_Event, params object[] a_Params)
    {
        m_PreviousTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    private void OnUnPauseGame(Event a_Event, params object[] a_Params)
    {
        Time.timeScale = m_PreviousTimeScale;
    }

}
