using UnityEngine;
using System.Collections;

using BennyBroseph;
using Define;

using Event = Define.Event;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            Publisher.self.Subscribe(Event.Instructions, OnInstructions);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnInstructions(Event a_Event, params object[] a_Params)
        {
            // Do stuff...
        }

        private void OnInstructionsClick()
        {
            Publisher.self.Broadcast(Event.Instructions, null);
        }
    }
}