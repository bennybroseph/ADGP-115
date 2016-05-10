using UnityEngine;
using UnityEngine.SceneManagement;
using Library;

using Event = Define.Event;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        //Function for NewGame button
        public void NewGame()
        {
            //Loads the first scene.
            SceneManager.LoadScene(1);
            //Publisher Subscriber for NewGame / Broadcast null
            Publisher.self.Broadcast(Event.NewGame, null);

        }
        //Function for LoadGame button
        public void LoadGame()
        {
            //Load Game Function
            //Publisher Subscriber for LoadGame/ Broadcast null
            Publisher.self.Broadcast(Event.LoadGame, null);
        }
        //Function for Instructions
        public void Instructions()
        {
            //Instructions Function.
            //Publisher Subscriber for Instructions / Broadcast null
            Publisher.self.Broadcast(Event.Instructions, null);
        }
        //Function for QuitGame button
        public void QuitGame()
        {
            //Used to Quit Application
            Application.Quit();
            //Publisher Subscriber or QuitGame / Broadcast null
            Publisher.self.Broadcast(Event.QuitGame, null);
        }
    }
}