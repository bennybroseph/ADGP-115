using UnityEngine;
using System.Collections.Generic;
#if !UNITY_WEBGL
using XInputDotNetPure; // Required in C#
#endif
using UnityEngine.SceneManagement;

using Library;
using UI;
using Units;
using Event = Define.Event;


public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private UIManager m_UIManager;
    [SerializeField]
    private float m_PreviousTimeScale;

    private GameObject m_Background;

#if !UNITY_WEBGL 
    private bool m_PlayerIndexSet;
    [SerializeField]
    private PlayerIndex m_PlayerIndex;
    private GamePadState m_State;
    private GamePadState m_PrevState;
#endif

    #region -- UNITY FUNCTIONS --
    private void Awake()
    {
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
    #endregion

    #region -- PUBLIC FUNCTIONS --

    public bool GetButtonState(PlayerIndex a_PlayerIndex, ButtonCode a_Button)
    {
        switch (a_Button)
        {
            case ButtonCode.A:
                return GamePad.GetState(a_PlayerIndex).Buttons.A == ButtonState.Pressed;
            case ButtonCode.B:
                return GamePad.GetState(a_PlayerIndex).Buttons.B == ButtonState.Pressed;
            case ButtonCode.X:
                return GamePad.GetState(a_PlayerIndex).Buttons.X == ButtonState.Pressed;
            case ButtonCode.Y:
                return GamePad.GetState(a_PlayerIndex).Buttons.Y == ButtonState.Pressed;

            case ButtonCode.DpadUp:
                return GamePad.GetState(a_PlayerIndex).DPad.Up == ButtonState.Pressed;
            case ButtonCode.DpadDown:
                return GamePad.GetState(a_PlayerIndex).DPad.Down == ButtonState.Pressed;
            case ButtonCode.DpadLeft:
                return GamePad.GetState(a_PlayerIndex).DPad.Left == ButtonState.Pressed;
            case ButtonCode.DpadRight:
                return GamePad.GetState(a_PlayerIndex).DPad.Right == ButtonState.Pressed;

            case ButtonCode.Start:
                return GamePad.GetState(a_PlayerIndex).Buttons.Start == ButtonState.Pressed;
            case ButtonCode.Back:
                return GamePad.GetState(a_PlayerIndex).Buttons.Back == ButtonState.Pressed;

            case ButtonCode.LBumper:
                return GamePad.GetState(a_PlayerIndex).Buttons.LeftShoulder == ButtonState.Pressed;
            case ButtonCode.RBumper:
                return GamePad.GetState(a_PlayerIndex).Buttons.RightShoulder == ButtonState.Pressed;
            case ButtonCode.LStick:
                return GamePad.GetState(a_PlayerIndex).Buttons.LeftStick == ButtonState.Pressed;
            case ButtonCode.RStick:
                return GamePad.GetState(a_PlayerIndex).Buttons.RightStick == ButtonState.Pressed;

            default:
                return false;
        }
    }
    #endregion

    #region -- EVENT FUNCTIONS --
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
    #endregion
}
