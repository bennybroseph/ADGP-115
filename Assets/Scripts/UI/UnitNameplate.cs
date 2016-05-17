using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Units;
using UnityEngine;
using UnityEngine.UI;

using Library;

using Event = Define.Event;

namespace UI
{
    public class UnitNameplate : MonoBehaviour, IChildable<IStats>
    {
        [SerializeField]
        private IStats m_Parent;

        [SerializeField]
        private Camera m_Camera;

        [SerializeField]
        private Vector3 m_Offset;

        [SerializeField]
        private Text m_NameText;
        [SerializeField]
        private Text m_LevelText;
        [SerializeField]
        private RectTransform m_HealthBar;
        [SerializeField]
        private RectTransform m_ManaBar;

        private Vector2 m_HealthBarOriginalSize;
        private Vector2 m_ManaBarOriginalSize;

        public IStats parent
        {
            get { return m_Parent; }
            set { m_Parent = value; Awake(); }
        }

        public void Awake()
        {
            if (m_Parent == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            transform.parent = UIManager.self.transform;
            transform.localScale = new Vector3(1, 1, 1);

            GetComponents();

            Publisher.self.Subscribe(Event.UnitInitialized, OnInit);

            Publisher.self.Subscribe(Event.UnitHealthChanged, OnValueChanged);
            Publisher.self.Subscribe(Event.UnitManaChanged, OnValueChanged);
            Publisher.self.Subscribe(Event.UnitLevelChanged, OnValueChanged);

            Publisher.self.Subscribe(Event.UnitDied, OnUnitDied);
        }

        // Use this for initialization
        void Start()
        {
            m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            Vector3 worldPos = m_Parent.transform.position + m_Offset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
        }

        private void GetComponents()
        {
            foreach (Transform child in transform)
            {
                switch (child.tag)
                {
                    case "Name Text":
                        {
                            if (m_NameText == null)
                                m_NameText = child.gameObject.GetComponent<Text>();
                        }
                        break;
                    case "Level Text":
                        {
                            if (m_LevelText == null)
                                m_LevelText = child.gameObject.GetComponent<Text>();
                        }
                        break;
                    case "Health Bar":
                        {
                            if (m_HealthBar == null)
                                m_HealthBar = child.gameObject.GetComponent<RectTransform>();
                        }
                        break;
                    case "Mana Bar":
                        {
                            if (m_ManaBar == null)
                                m_ManaBar = child.gameObject.GetComponent<RectTransform>();
                        }
                        break;
                }
            }
        }

        private void OnInit(Event a_Event, params object[] a_Params)
        {
            IStats unit = a_Params[0] as IStats;

            if (unit == null || unit != m_Parent)
                return;

            SetText(m_NameText, unit.unitNickname);
            SetText(m_LevelText, unit.level.ToString(), true);
            SetBar(m_HealthBar, unit.health, unit.maxHealth);
            SetBar(m_ManaBar, unit.mana, unit.maxMana);
        }

        private void OnValueChanged(Event a_Event, params object[] a_Params)
        {
            IStats unit = a_Params[0] as IStats;

            if (unit == null || unit != m_Parent)
                return;

            switch (a_Event)
            {
                case Event.UnitLevelChanged:
                    SetText(m_LevelText, unit.level.ToString(), true);
                    break;
                case Event.UnitHealthChanged:
                    SetBar(m_HealthBar, unit.health, unit.maxHealth);
                    break;
                case Event.UnitManaChanged:
                    SetBar(m_ManaBar, unit.mana, unit.maxMana);
                    break;
            }
        }

        private void SetText(Text a_Text, string a_String, bool a_AddLevel = false)
        {
            if (a_Text == null)
                return;

            if (a_AddLevel)
                a_Text.text = "lvl. " + a_String;
            else
                a_Text.text = a_String;
        }

        private void SetBar(RectTransform a_Bar, float a_CurrentValue, float a_MaxValue)
        {
            if (a_Bar == null)
                return;

            a_Bar.GetComponent<Image>().fillAmount = a_CurrentValue / a_MaxValue;

            if (a_Bar.GetComponentInChildren<Text>() == null)
                return;
            a_Bar.GetComponentInChildren<Text>().text = a_CurrentValue + "/" + a_MaxValue;
        }

        private void OnUnitDied(Event a_Event, params object[] a_Params)
        {
            IStats unit = a_Params[0] as IStats;

            // The 'this == null' call is surprisingly necessary when in the editor...
            if (unit == null || unit != m_Parent || this == null)
                return;

            Destroy(gameObject);
        }
    }
}
