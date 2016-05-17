using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Interfaces;
using Units;
using UnityEngine;
using UnityEngine.UI;

using Library;

using Event = Define.Event;

namespace UI
{
    public class UnitNameplate : MonoBehaviour, IChildable<IStats>
    {
        #region -- ENUM -- 
        private enum BarType
        {
            Health,
            Mana,
        }
        #endregion

        #region -- PRIVATE VARIABLES --
        [SerializeField]
        private IStats m_Parent;

        [SerializeField]
        private Vector3 m_Offset;

        [SerializeField]
        private Text m_NameText;
        [SerializeField]
        private Text m_LevelText;
        [SerializeField]
        private Image m_HealthBar;
        [SerializeField]
        private Image m_ManaBar;
        [SerializeField]
        private Image m_NegativeHealthBar;
        [SerializeField]
        private Image m_NegativeManaBar;

        [SerializeField]
        private float m_LastHealthChange;
        [SerializeField]
        private bool m_HealthCoroutineIsRunning;

        [SerializeField]
        private float m_LastManaChange;
        [SerializeField]
        private bool m_ManaCoroutineIsRunning;
        #endregion

        #region -- ISTATS PARENT --
        public IStats parent
        {
            get { return m_Parent; }
            set { m_Parent = value; Awake(); }
        }
        #endregion

        #region -- UNITY FUNCTIONS --
        public void Awake()
        {
            if (m_Parent == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            transform.SetParent(UIManager.self.transform);
            transform.localScale = new Vector3(1, 1, 1);

            transform.SetAsFirstSibling();

            GetComponents();

            m_LastHealthChange = 0;
            m_HealthCoroutineIsRunning = false;

            m_LastManaChange = 0;
            m_ManaCoroutineIsRunning = false;

            Publisher.self.Subscribe(Event.UnitInitialized, OnInit);

            Publisher.self.Subscribe(Event.UnitHealthChanged, OnValueChanged);
            Publisher.self.Subscribe(Event.UnitManaChanged, OnValueChanged);
            Publisher.self.Subscribe(Event.UnitLevelChanged, OnValueChanged);

            Publisher.self.Subscribe(Event.UnitDied, OnUnitDied);
        }

        // Use this for initialization
        void Start()
        {

        }

        private void LateUpdate()
        {
            Vector3 worldPos = m_Parent.transform.position + m_Offset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
        }
        #endregion

        #region -- PRIVATE VOID FUNCTIONS --
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
                                m_HealthBar = child.gameObject.GetComponent<Image>();
                        }
                        break;
                    case "Mana Bar":
                        {
                            if (m_ManaBar == null)
                                m_ManaBar = child.gameObject.GetComponent<Image>();
                        }
                        break;
                    case "Negative Health Bar":
                        {
                            if (m_NegativeHealthBar == null)
                                m_NegativeHealthBar = child.gameObject.GetComponent<Image>();
                        }
                        break;
                    case "Negative Mana Bar":
                        {
                            if (m_NegativeManaBar == null)
                                m_NegativeManaBar = child.gameObject.GetComponent<Image>();
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

                    if (m_NegativeHealthBar.fillAmount < m_HealthBar.fillAmount)
                        m_NegativeHealthBar.fillAmount = m_HealthBar.fillAmount;

                    m_LastHealthChange = 0;
                    if (!m_HealthCoroutineIsRunning)
                        StartCoroutine(ReduceNegativeHealthSpace());
                    break;
                case Event.UnitManaChanged:
                    SetBar(m_ManaBar, unit.mana, unit.maxMana);

                    if (m_NegativeManaBar.fillAmount < m_ManaBar.fillAmount)
                        m_NegativeManaBar.fillAmount = m_ManaBar.fillAmount;

                    m_LastManaChange = 0;
                    if (!m_ManaCoroutineIsRunning)
                        StartCoroutine(ReduceNegativeManaSpace());
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

        private void SetBar(Image a_Bar, float a_CurrentValue, float a_MaxValue)
        {
            if (a_Bar == null)
                return;

            a_Bar.fillAmount = a_CurrentValue / a_MaxValue;

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
#endregion

        #region -- ENUMERATOR --
        private IEnumerator ReduceNegativeHealthSpace()
        {
            m_HealthCoroutineIsRunning = true;
            while (m_HealthBar.fillAmount != m_NegativeHealthBar.fillAmount)
            {
                while (m_LastHealthChange < 1.5f)
                {
                    m_LastHealthChange += Time.deltaTime;
                    yield return false;
                }

                m_LastHealthChange += Time.deltaTime;
                m_NegativeHealthBar.fillAmount -= Mathf.Sqrt(m_LastHealthChange - 1.5f) * Time.deltaTime;

                if (m_NegativeHealthBar.fillAmount < m_HealthBar.fillAmount)
                    m_NegativeHealthBar.fillAmount = m_HealthBar.fillAmount;

                yield return false;
            }
            m_HealthCoroutineIsRunning = false;
        }
        private IEnumerator ReduceNegativeManaSpace()
        {
            m_ManaCoroutineIsRunning = true;
            while (m_ManaBar.fillAmount != m_NegativeManaBar.fillAmount)
            {
                while (m_LastManaChange < 1.5f)
                {
                    m_LastManaChange += Time.deltaTime;
                    yield return false;
                }

                m_LastManaChange += Time.deltaTime;
                m_NegativeManaBar.fillAmount -= Mathf.Sqrt(m_LastManaChange - 1.5f) * Time.deltaTime;

                if (m_NegativeManaBar.fillAmount < m_ManaBar.fillAmount)
                    m_NegativeManaBar.fillAmount = m_ManaBar.fillAmount;

                yield return false;
            }
            m_ManaCoroutineIsRunning = false;
        }
#endregion
    }
}
