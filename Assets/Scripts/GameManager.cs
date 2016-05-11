using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Library;
using UnityEditor;
using UnityEngine.SceneManagement;
using Event = Define.Event;


public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private GameObject m_QuitMenu;

    [SerializeField]
    private float m_PreviousTimeScale;

    private List<Node> m_SearchSpace;

    private GameObject m_Background;

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
