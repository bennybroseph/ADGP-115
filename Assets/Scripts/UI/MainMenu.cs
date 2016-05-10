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
    }
}