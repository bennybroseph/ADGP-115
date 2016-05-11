using System;
using UnityEngine;
using System.Collections;
using Library;
using UnityEngine.SceneManagement;
using Event = Define.Event;


public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private GameObject m_QuitMenu;

    [SerializeField]
    private float m_PreviousTimeScale;

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_QuitMenu.SetActive(!m_QuitMenu.activeInHierarchy);

            Publisher.self.Broadcast((m_QuitMenu.activeInHierarchy) ? Event.PauseGame : Event.UnPauseGame);
        }
        if(Input.GetKeyDown(KeyCode.Q))
            Publisher.self.Broadcast(Event.UseSkill, 1);
        if(Input.GetKeyDown(KeyCode.E))
            Publisher.self.Broadcast(Event.UseSkill, 2);
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
