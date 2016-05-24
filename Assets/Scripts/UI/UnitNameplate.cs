using System;
using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

using Library;

using Random = UnityEngine.Random;
using Event = Define.Event;

namespace UI
{
    public class UnitNameplate : MonoBehaviour, IChildable<IStats>
    {
        private delegate void VoidFunction();

        #region -- PRIVATE VARIABLES --
        [SerializeField]
        private IStats m_Parent;

        [SerializeField]
        private Vector3 m_Offset;

        [SerializeField]
        private AnimationCurve m_BarAnimationCurve;

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

        #region -- PROPERTIES --
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

            transform.SetParent(UIManager.self.backgroundUI.transform, false);

            transform.SetAsFirstSibling();

            GetComponents();

            m_LastHealthChange = 0;
            m_HealthCoroutineIsRunning = false;

            m_LastManaChange = 0;
            m_ManaCoroutineIsRunning = false;

            Publisher.self.Subscribe(Event.UnitInitialized, OnInit);

            Publisher.self.Subscribe(Event.UnitMaxHealthChanged, OnValueChanged);
            Publisher.self.Subscribe(Event.UnitMaxHealthChanged, OnValueChanged);

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
            Vector3 worldPos = m_Parent.gameObject.transform.position + m_Offset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
        }
        #endregion

        #region -- PRIVATE FUNCTIONS --
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

        #region -- EVENTS --
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

                case Event.UnitMaxHealthChanged:
                    SetBar(m_HealthBar, unit.health, unit.maxHealth);
                    break;
                case Event.UnitHealthChanged:
                    {
                        SetBar(m_HealthBar, unit.health, unit.maxHealth);

                        if (m_NegativeHealthBar.fillAmount < m_HealthBar.fillAmount)
                            m_NegativeHealthBar.fillAmount = m_HealthBar.fillAmount;

                        m_LastHealthChange = 0;
                        if (!m_HealthCoroutineIsRunning)
                            StartCoroutine(ReduceNegativeHealthSpace());
                    }
                    break;

                case Event.UnitMaxManaChanged:
                    SetBar(m_ManaBar, unit.mana, unit.maxMana);
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
        private void OnUnitDied(Event a_Event, params object[] a_Params)
        {
            IStats unit = a_Params[0] as IStats;

            // The 'this == null' call is surprisingly necessary when in the editor...
            if (unit == null || unit != m_Parent || this == null)
                return;

            Publisher.self.UnSubscribe(Event.UnitInitialized, OnInit);

            Publisher.self.UnSubscribe(Event.UnitHealthChanged, OnValueChanged);
            Publisher.self.UnSubscribe(Event.UnitManaChanged, OnValueChanged);
            Publisher.self.UnSubscribe(Event.UnitLevelChanged, OnValueChanged);

            Publisher.self.UnSubscribe(Event.UnitDied, OnUnitDied);

            Destroy(gameObject);
        }
        #endregion

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
            a_Bar.GetComponentInChildren<Text>().text = 
                Math.Ceiling(a_CurrentValue) + "/" + Math.Ceiling(a_MaxValue);
        }
        #endregion

        #region -- COROUTINES --
        private IEnumerator ReduceNegativeHealthSpace()
        {
            m_HealthCoroutineIsRunning = true;

            float amountToReduce = 0;
            float originalFill = m_NegativeHealthBar.fillAmount;

            float deltaTime = 0;
            while (deltaTime < m_BarAnimationCurve[m_BarAnimationCurve.length - 1].time)
            {
                while (m_LastHealthChange < 0.5f)
                {
                    m_LastHealthChange += Time.deltaTime;

                    deltaTime = 0;

                    amountToReduce = m_NegativeHealthBar.fillAmount - m_HealthBar.fillAmount;
                    originalFill = m_NegativeHealthBar.fillAmount;

                    yield return false;
                }

                deltaTime += Time.deltaTime;

                m_NegativeHealthBar.fillAmount =
                    originalFill
                    - m_BarAnimationCurve.Evaluate(deltaTime)
                    * amountToReduce;

                yield return false;
            }
            m_HealthCoroutineIsRunning = false;
        }
        private IEnumerator ReduceNegativeManaSpace()
        {
            m_ManaCoroutineIsRunning = true;

            float amountToReduce = 0;
            float originalFill = m_NegativeManaBar.fillAmount;

            float deltaTime = 0;
            while (deltaTime < m_BarAnimationCurve[m_BarAnimationCurve.length - 1].time)
            {
                while (m_LastManaChange < 0.25f)
                {
                    m_LastManaChange += Time.deltaTime;

                    deltaTime = 0;

                    amountToReduce = m_NegativeManaBar.fillAmount - m_ManaBar.fillAmount;
                    originalFill = m_NegativeManaBar.fillAmount;

                    yield return false;
                }

                deltaTime += Time.deltaTime;
                m_NegativeManaBar.fillAmount = originalFill
                    - m_BarAnimationCurve.Evaluate(deltaTime)
                    * amountToReduce;

                yield return false;
            }
            m_ManaCoroutineIsRunning = false;
        }

        private IEnumerator WaitAndDoThis(
            float a_TimeToWait,
            VoidFunction a_Delegate,
            bool a_CallDelegateFirst = false)
        {
            if (a_CallDelegateFirst)
                a_Delegate();

            yield return new WaitForSeconds(a_TimeToWait);

            if (!a_CallDelegateFirst)
                a_Delegate();
        }
        #endregion
    }
}
