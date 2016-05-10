using UnityEngine;
using System.Collections;
using Library;
using UnityEngine.SceneManagement;
using Event = Define.Event;


public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private GameObject m_QuitMenu;

    // Use this for initialization
    private void Start()
    {
        Publisher.self.Subscribe(Event.QuitGame, OnQuitGame);
        Publisher.self.Subscribe(Event.NewGame, OnNewGame);
    }

    // Update is called once per frame
    private void Update()
    {
        Publisher.self.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_QuitMenu.SetActive(true);
            Time.timeScale = 0;
            Publisher.self.Broadcast(Event.PauseGame);
        }
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
}
