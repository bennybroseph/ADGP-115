using System;
using System.Collections.Generic;
using Units;
using UnityEngine;
using UnityEngine.UI;

using Library;

using Event = Define.Event;

namespace UI
{
    public class UnitNameplate : MonoBehaviour, IChildable<IStats>
    {
        [Serializable]
        private class CanvasObject<TComponent> where TComponent : Component
        {
            private TComponent m_Component;
            private Vector3 m_OriginalPosition;

            public TComponent component
            {
                get { return m_Component; }
                set { m_Component = value; }
            }

            public Vector3 originalPosition
            {
                get { return m_OriginalPosition; }
                set { m_OriginalPosition = value; }
            }

            public Transform transform
            {
                get { return m_Component.transform; }
            }

            public CanvasObject()
            {
                m_Component = null;
                m_OriginalPosition = new Vector3();
            }

            public CanvasObject(TComponent a_Component, Vector3 a_OriginalPosition)
            {
                m_Component = a_Component;
                m_OriginalPosition = a_OriginalPosition;
            }
        }

        [SerializeField]
        private IStats m_Parent;

        [SerializeField]
        private Camera m_Camera;

        [SerializeField]
        private Vector3 m_Offset;

        [SerializeField]
        private List<CanvasObject<Component>> m_CanvasObjects;

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

            m_CanvasObjects = new List<CanvasObject<Component>>();
            foreach (Transform child in transform)
            {
                m_CanvasObjects.Add(new CanvasObject<Component>(child.GetComponent<Component>(), child.transform.position));
            }

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
            GetComponent<Canvas>().worldCamera = m_Camera;
        }

        private void LateUpdate()
        {
            //transform.position = m_Parent.transform.position + m_Offset;

            Vector3 relativePosition = Camera.main.WorldToScreenPoint(parent.transform.position);

            foreach (CanvasObject<Component> canvasObject in m_CanvasObjects)
            {
                canvasObject.transform.position = relativePosition + m_Offset + canvasObject.originalPosition - (Vector3)(canvasObject.component.GetComponent<RectTransform>().sizeDelta * GetComponent<Canvas>().scaleFactor / 2);
            }
            Debug.Log(GetComponent<Canvas>().scaleFactor);
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
