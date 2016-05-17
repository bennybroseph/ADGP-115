using System.Collections.Generic;
using Library;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAnnouncer : MonoSingleton<UIAnnouncer>
    {
        [SerializeField]
        private List<string> m_Announcments;

        // Use this for initialization
        private void Awake()
        {
            m_Announcments = new List<string>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
