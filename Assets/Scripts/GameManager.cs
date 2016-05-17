using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

#if !UNITY_WEBGL
using XInputDotNetPure; // Required in C#
#endif

using Library;
using UI;
using Units.Controller;

using Event = Define.Event;


public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private UIManager m_UIManager;
    [SerializeField]
    private float m_PreviousTimeScale;
    [SerializeField]
    private List<IAttackable> m_Fortresses;

    private GameObject m_Background;

#if !UNITY_WEBGL 
    [SerializeField]
    private List<PlayerIndex> m_PlayerIndices;
#endif

    #region -- UNITY FUNCTIONS --
    protected override void Awake()
    {
        base.Awake();

        Instantiate(m_UIManager);
        m_Fortresses = new List<IAttackable>();
        m_PreviousTimeScale = Time.timeScale;

        Application.targetFrameRate = -1;

        Publisher.self.Subscribe(Event.NewGame, OnNewGame);
        Publisher.self.Subscribe(Event.QuitGame, OnQuitGame);

        Publisher.self.Subscribe(Event.PauseGame, OnPauseGame);
        Publisher.self.Subscribe(Event.UnPauseGame, OnUnPauseGame);
        Publisher.self.Subscribe(Event.FortressInitialized, OnFortressInit);
        Publisher.self.Subscribe(Event.FortressDied, OnFortressDied);
    }

    // Update is called once per frame
    private void Update()
    {
        Publisher.self.Update();

#if !UNITY_WEBGL
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        for (int i = 0; i < 4; ++i)
        {
            PlayerIndex testPlayerIndex = (PlayerIndex)i;
            GamePadState testState = GamePad.GetState(testPlayerIndex);
            if (testState.IsConnected && !m_PlayerIndices.Contains(testPlayerIndex))
            {
                Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                m_PlayerIndices.Add(testPlayerIndex);
                m_PlayerIndices.Sort();
            }
            else if (!testState.IsConnected && m_PlayerIndices.Contains(testPlayerIndex))
            {
                Debug.Log(string.Format("GamePad removed {0}", testPlayerIndex));
                m_PlayerIndices.Remove(testPlayerIndex);
                m_PlayerIndices.Sort();
            }
        }
#endif

        if (Input.GetKeyDown(KeyCode.P))
            Publisher.self.Broadcast(Event.ToggleQuitMenu);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Publisher.self.UnSubscribe(Event.NewGame, OnNewGame);
        Publisher.self.UnSubscribe(Event.QuitGame, OnQuitGame);

        Publisher.self.UnSubscribe(Event.PauseGame, OnPauseGame);
        Publisher.self.UnSubscribe(Event.UnPauseGame, OnUnPauseGame);
        Publisher.self.UnSubscribe(Event.FortressInitialized, OnFortressInit);
        Publisher.self.UnSubscribe(Event.FortressDied, OnFortressDied);
    }
    #endregion

    #region -- PUBLIC FUNCTIONS --
#if !UNITY_WEBGL
    public bool GetButtonState(int a_Index, ButtonCode a_Button)
    {
        if (m_PlayerIndices.Count <= a_Index)
            return false;

        switch (a_Button)
        {
            case ButtonCode.A:
                return GamePad.GetState(m_PlayerIndices[a_Index]).Buttons.A == ButtonState.Pressed;
            case ButtonCode.B:
                return GamePad.GetState(m_PlayerIndices[a_Index]).Buttons.B == ButtonState.Pressed;
            case ButtonCode.X:
                return GamePad.GetState(m_PlayerIndices[a_Index]).Buttons.X == ButtonState.Pressed;
            case ButtonCode.Y:
                return GamePad.GetState(m_PlayerIndices[a_Index]).Buttons.Y == ButtonState.Pressed;

            case ButtonCode.DpadUp:
                return GamePad.GetState(m_PlayerIndices[a_Index]).DPad.Up == ButtonState.Pressed;
            case ButtonCode.DpadDown:
                return GamePad.GetState(m_PlayerIndices[a_Index]).DPad.Down == ButtonState.Pressed;
            case ButtonCode.DpadLeft:
                return GamePad.GetState(m_PlayerIndices[a_Index]).DPad.Left == ButtonState.Pressed;
            case ButtonCode.DpadRight:
                return GamePad.GetState(m_PlayerIndices[a_Index]).DPad.Right == ButtonState.Pressed;

            case ButtonCode.Start:
                return GamePad.GetState(m_PlayerIndices[a_Index]).Buttons.Start == ButtonState.Pressed;
            case ButtonCode.Back:
                return GamePad.GetState(m_PlayerIndices[a_Index]).Buttons.Back == ButtonState.Pressed;

            case ButtonCode.LBumper:
                return GamePad.GetState(m_PlayerIndices[a_Index]).Buttons.LeftShoulder == ButtonState.Pressed;
            case ButtonCode.RBumper:
                return GamePad.GetState(m_PlayerIndices[a_Index]).Buttons.RightShoulder == ButtonState.Pressed;
            case ButtonCode.LStick:
                return GamePad.GetState(m_PlayerIndices[a_Index]).Buttons.LeftStick == ButtonState.Pressed;
            case ButtonCode.RStick:
                return GamePad.GetState(m_PlayerIndices[a_Index]).Buttons.RightStick == ButtonState.Pressed;

            default:
                return false;
        }
    }

    public enum Stick
    {
        Left,
        Right,
    };

    public GamePadThumbSticks.StickValue GetStickValue(int a_Index, Stick a_Stick)
    {
        if (m_PlayerIndices.Count <= a_Index)
            return new GamePadThumbSticks.StickValue();

        switch (a_Stick)
        {
            case Stick.Left:
                return GamePad.GetState(m_PlayerIndices[a_Index]).ThumbSticks.Left;
            case Stick.Right:
                return GamePad.GetState(m_PlayerIndices[a_Index]).ThumbSticks.Right;

            default:
                return new GamePadThumbSticks.StickValue();
        }
    }
#endif
    #endregion

    #region -- EVENT FUNCTIONS --
    private void OnNewGame(Event a_Event, params object[] a_Params)
    {
        Debug.Log("Works New Game Clicked");
        //Loads the first scene.
        SceneManager.LoadScene(1);
        
    }

    private void OnQuitGame(Event a_Event, params object[] a_Params)
    {
        Debug.Log("Works Quit Game Clicked");
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

    private void OnFortressDied(Event a_Event, params object[] a_Params)
    {
        IAttackable fortress = a_Params[0] as IAttackable;
        m_Fortresses.Remove(fortress);

        if (m_Fortresses.Count == 0)
            Publisher.self.Broadcast(Event.GameOver);
    }

    private void OnFortressInit(Event a_Event, params object[] a_Params)
    {
        IAttackable fortress = a_Params[0] as IAttackable;
        m_Fortresses.Add(fortress);
    }


    #endregion
}
