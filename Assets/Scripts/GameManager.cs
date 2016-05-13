using UnityEngine;
using System.Collections.Generic;
using XInputDotNetPure; // Required in C#
using UnityEngine.SceneManagement;

using Library;
using UI;
using Event = Define.Event;


public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private UIManager m_UIManager;
    [SerializeField]
    private float m_PreviousTimeScale;

    private bool m_PlayerIndexSet = false;
    [SerializeField]
    private PlayerIndex m_PlayerIndex;
    private GamePadState m_State;
    private GamePadState m_PrevState;

    private List<Node> m_SearchSpace;

    private GameObject m_Background;

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

    private void Awake()
    {
        Instantiate(m_UIManager);
    }
    // Use this for initialization
    private void Start()
    {
        m_SearchSpace = SetUpSearchSpace();
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
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    m_PlayerIndex = testPlayerIndex;
                    m_PlayerIndexSet = true;
                }
            }
        }

        m_PrevState = m_State;
        m_State = GamePad.GetState(m_PlayerIndex);

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

    private List<Node> SetUpSearchSpace()
    {
        List<Node> searchSpace = new List<Node>();

        m_Background = GameObject.FindGameObjectWithTag("Background");

        Vector3 grid = new Vector3(m_Background.transform.localScale.x, m_Background.transform.localScale.y, m_Background.transform.localScale.z);

        int id = 0;

        //Base off x and z position
        int rows = 10;
        int columns = 10;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                Node node = new Node(x, y, id);

                searchSpace.Add(node);

                id += 1;
            }
        }

        return searchSpace;
    }

}
