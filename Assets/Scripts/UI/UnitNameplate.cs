using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

using Library;
using Units;
using Event = Define.Event;

namespace UI
{
    public class UnitNameplate : MonoBehaviour, IChildable<IStats>, IChildable<Fortress>
    {
        private delegate void VoidFunction();

        #region -- VARIABLES --
        [Header("Components")]
        [SerializeField]
        private Text m_NameText;
        [SerializeField]
        private Text m_LevelText;
        [SerializeField]
        private Image m_NegativeEXPBar;
        [SerializeField]
        private Image m_EXPBar;
        [SerializeField]
        private Image m_NegativeManaBar;
        [SerializeField]
        private Image m_ManaBar;
        [SerializeField]
        private Image m_NegativeHealthBar;
        [SerializeField]
        private Image m_HealthBar;

        [Space]
        [SerializeField]
        private Vector3 m_Offset;
        [SerializeField]
        private Vector2 m_CollisionOffset;
        [SerializeField]
        private bool m_IsColliding;

        [SerializeField]
        private AnimationCurve m_BarAnimationCurve;

        [SerializeField]
        private float m_LastHealthChange;
        [SerializeField]
        private bool m_HealthCoroutineIsRunning;

        [SerializeField]
        private float m_LastManaChange;
        [SerializeField]
        private bool m_ManaCoroutineIsRunning;

        private IStats m_StatParent;
        private Fortress m_FortressParent;

        private List<Coroutine> m_FadeCoroutines;

        private bool m_CurrentlyTargeted;
        private float m_CanvasAlpha;

        [SerializeField]
        private bool m_IsStationary;
        [SerializeField]
        private bool m_HideWhenInactive;
        #endregion

        #region -- PROPERTIES --
        public IStats parent
        {
            get { return m_StatParent; }
            set { m_StatParent = value; }
        }

        public Fortress fortressParent
        {
            get { return m_FortressParent; }
            set { m_FortressParent = value; }
        }
        Fortress IChildable<Fortress>.parent
        {
            get { return m_FortressParent; }
            set { m_FortressParent = value; }
        }

        #endregion

        #region -- UNITY FUNCTIONS --
        private void Awake()
        {
            transform.SetParent(UIManager.self.backgroundUI.transform, false);

            transform.SetAsFirstSibling();

            m_FadeCoroutines = new List<Coroutine>();

            m_LastHealthChange = 0;
            m_HealthCoroutineIsRunning = false;

            m_LastManaChange = 0;
            m_ManaCoroutineIsRunning = false;

            // These are not the only subscriptions, just the only ones we care about at the start
            Publisher.self.Subscribe(Event.PlayerTargetChanged, OnPlayerTargetChanged);

            Publisher.self.Subscribe(Event.FortressInitialized, OnFortressInit);
            Publisher.self.Subscribe(Event.FortressDied, OnFortressDied);

            Publisher.self.Subscribe(Event.UnitInitialized, OnUnitInit);
            Publisher.self.Subscribe(Event.UnitDied, OnUnitDied);
        }

        private void Start()
        {
            if (m_HideWhenInactive)
                SetAlpha(0.0f);
        }

        private void LateUpdate()
        {
            if (m_IsStationary)
                return;

            GameObject tempParent = m_StatParent != null ? m_StatParent.gameObject : m_FortressParent.gameObject;

            Vector3 worldPos = tempParent.gameObject.transform.position + m_Offset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            transform.position = new Vector3(screenPos.x + m_CollisionOffset.x, screenPos.y + m_CollisionOffset.y);

            if (m_IsColliding == false)
                m_CollisionOffset = Vector2.Lerp(m_CollisionOffset, Vector2.zero, Time.deltaTime / 2f);
        }

        private void OnCollisionEnter2D(Collision2D a_Collision)
        {
            m_IsColliding = true;
        }

        private void OnCollisionStay2D(Collision2D a_Collision)
        {
            foreach (ContactPoint2D contactPoint2D in a_Collision.contacts)
            {
                m_CollisionOffset += Vector2.Lerp(Vector2.zero, contactPoint2D.normal, 0.2f);
            }
        }
        private void OnCollisionExit2D(Collision2D a_Collision)
        {
            m_IsColliding = false;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();

            Publisher.self.UnSubscribe(Event.PlayerTargetChanged, OnPlayerTargetChanged);

            Publisher.self.UnSubscribe(Event.FortressInitialized, OnFortressInit);

            Publisher.self.UnSubscribe(Event.FortressHealthChanged, OnFortressValueChanged);

            Publisher.self.UnSubscribe(Event.FortressDied, OnFortressDied);

            Publisher.self.UnSubscribe(Event.UnitInitialized, OnUnitInit);

            Publisher.self.UnSubscribe(Event.UnitMaxHealthChanged, OnUnitValueChanged);
            Publisher.self.UnSubscribe(Event.UnitMaxHealthChanged, OnUnitValueChanged);

            Publisher.self.UnSubscribe(Event.UnitEXPChanged, OnUnitValueChanged);

            Publisher.self.UnSubscribe(Event.UnitHealthChanged, OnUnitValueChanged);
            Publisher.self.UnSubscribe(Event.UnitManaChanged, OnUnitValueChanged);
            Publisher.self.UnSubscribe(Event.UnitLevelChanged, OnUnitValueChanged);

            Publisher.self.UnSubscribe(Event.UnitDied, OnUnitDied);
        }
        #endregion

        #region -- PRIVATE FUNCTIONS --
        private void SetAlpha(float a_FadeTo)
        {
            foreach (Graphic graphic in GetComponentsInChildren<Graphic>())
                graphic.canvasRenderer.SetAlpha(a_FadeTo);

            if (a_FadeTo == 0f)
                gameObject.SetActive(false);
        }
        private void Fade(float a_FadeTo, float a_Duration)
        {
            foreach (Coroutine fadeCoroutine in m_FadeCoroutines)
                StopCoroutine(fadeCoroutine);

            foreach (Graphic graphic in GetComponentsInChildren<Graphic>())
                graphic.CrossFadeAlpha(a_FadeTo, a_Duration, false);
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

            a_Bar.fillAmount = Mathf.Ceil(a_CurrentValue) / Mathf.Ceil(a_MaxValue);

            if (a_Bar.GetComponentInChildren<Text>() == null)
                return;
            a_Bar.GetComponentInChildren<Text>().text =
                Math.Ceiling(a_CurrentValue) + "/" + Math.Ceiling(a_MaxValue);
        }

        private void OnInit(object a_Object)
        {
            IAttackable attackable = a_Object as IAttackable;

            if (attackable == null)
                return;

            SetText(m_NameText, attackable.unitNickname);
            SetBar(m_HealthBar, attackable.health, attackable.maxHealth);

            IStats unit = a_Object as IStats;

            if (unit == null)
                return;

            SetText(m_LevelText, unit.level.ToString(), true);
            SetBar(m_ManaBar, unit.mana, unit.maxMana);

            float lastLevel, nextLevel;
            unit.GetLevelBar(out lastLevel, out nextLevel);
            SetBar(m_EXPBar, unit.experience - lastLevel, nextLevel - lastLevel);
        }
        #endregion

        #region -- EVENTS --
        private void OnPlayerTargetChanged(Event a_Event, params object[] a_Params)
        {
            GameObject target = a_Params[1] as GameObject;

            if (!m_HideWhenInactive || this == null)
                return;

            IStats unit = null;
            if (target != null)
                unit = target.GetComponent<IStats>();

            if (!gameObject.activeInHierarchy && unit != m_StatParent)
                return;

            if (!gameObject.activeInHierarchy)
            {
                SetAlpha(0f);
                gameObject.SetActive(true);
            }

            if (unit == null || unit != m_StatParent)
            {
                m_CurrentlyTargeted = false;
                Fade(0f, 1.5f);
            }
            else
            {
                m_CurrentlyTargeted = true;
                Fade(1f, 0.5f);
            }
        }

        private void OnFortressInit(Event a_Event, params object[] a_Params)
        {
            Fortress fortress = a_Params[0] as Fortress;

            if (fortress == null || fortress != m_FortressParent)
                return;

            // Only subscribe once the fortress has been initialized
            Publisher.self.Subscribe(Event.FortressHealthChanged, OnFortressValueChanged);

            OnInit(fortress);
        }
        private void OnUnitInit(Event a_Event, params object[] a_Params)
        {
            IStats unit = a_Params[0] as IStats;

            if (unit == null || unit != m_StatParent)
                return;

            // Only subscribe once the unit has been initialized
            Publisher.self.Subscribe(Event.UnitMaxHealthChanged, OnUnitValueChanged);
            Publisher.self.Subscribe(Event.UnitMaxHealthChanged, OnUnitValueChanged);

            Publisher.self.Subscribe(Event.UnitEXPChanged, OnUnitValueChanged);

            Publisher.self.Subscribe(Event.UnitHealthChanged, OnUnitValueChanged);
            Publisher.self.Subscribe(Event.UnitManaChanged, OnUnitValueChanged);
            Publisher.self.Subscribe(Event.UnitLevelChanged, OnUnitValueChanged);

            OnInit(unit);
        }

        private void OnFortressValueChanged(Event a_Event, params object[] a_Params)
        {
            IAttackable fortress = a_Params[0] as IAttackable;

            if (fortress == null || fortress != m_FortressParent)
                return;

            switch (a_Event)
            {
                case Event.FortressHealthChanged:
                    SetBar(m_HealthBar, fortress.health, fortress.maxHealth);

                    if (m_NegativeHealthBar.fillAmount < m_HealthBar.fillAmount)
                        m_NegativeHealthBar.fillAmount = m_HealthBar.fillAmount;

                    m_LastHealthChange = 0;
                    if (!m_HealthCoroutineIsRunning)
                        StartCoroutine(ReduceNegativeHealthSpace());
                    break;
            }
        }
        private void OnUnitValueChanged(Event a_Event, params object[] a_Params)
        {
            IStats unit = a_Params[0] as IStats;

            if (unit == null || unit != m_StatParent)
                return;

            if (m_HideWhenInactive)
            {
                if (!gameObject.activeInHierarchy)
                {
                    SetAlpha(0f);
                    gameObject.SetActive(true);
                }
                if (a_Event != Event.UnitInitialized)
                {
                    Fade(1, 0.5f);
                    m_FadeCoroutines.Add(
                        StartCoroutine(
                            WaitAndDoThis(
                                2f,
                                delegate
                                {
                                    if (!m_CurrentlyTargeted)
                                        Fade(0f, 0.5f);
                                })));
                }
            }

            switch (a_Event)
            {
                case Event.UnitLevelChanged:
                    SetText(m_LevelText, unit.level.ToString(), true);
                    break;
                case Event.UnitEXPChanged:
                    float lastLevel, nextLevel;
                    unit.GetLevelBar(out lastLevel, out nextLevel);

                    SetBar(m_EXPBar, unit.experience - lastLevel, nextLevel - lastLevel);
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

        private void OnFortressDied(Event a_Event, params object[] a_Params)
        {
            Fortress fortress = a_Params[0] as Fortress;

            // The 'this == null' call is surprisingly necessary when in the editor...
            if (fortress == null || fortress != m_FortressParent || this == null)
                return;

            StopAllCoroutines();
            Destroy(gameObject);
        }
        private void OnUnitDied(Event a_Event, params object[] a_Params)
        {
            IStats unit = a_Params[0] as IStats;

            // The 'this == null' call is surprisingly necessary when in the editor...
            if (unit == null || unit != m_StatParent || this == null)
                return;

            StopAllCoroutines();
            Destroy(gameObject);
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
