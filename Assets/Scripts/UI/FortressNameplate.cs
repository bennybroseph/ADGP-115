using System.Collections;
using Interfaces;
using Library;
using Units;
using UnityEngine;
using UnityEngine.UI;
using Event = Define.Event;

namespace UI
{
    public class FortressNameplate : MonoBehaviour, IChildable<Fortress>
    {
        [SerializeField]
        private Fortress m_Parent;

        [SerializeField]
        private Vector3 m_Offset;

        [SerializeField]
        private Image m_HealthBar;
        [SerializeField]
        private Text m_HealthText;

        [SerializeField]
        private Image m_NegativeHealthBar;

        [SerializeField]
        private float m_LastHealthChange;
        [SerializeField]
        private bool m_HealthCoroutineIsRunning;

        public Fortress parent
        {
            get { return m_Parent; }
            set { m_Parent = value; Awake(); }
        }

        private void Awake()
        {
            if (m_Parent == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            transform.SetParent(UIManager.self.transform, false);
            transform.SetAsFirstSibling();

            foreach (Transform child in transform)
            {
                switch (child.gameObject.tag)
                {
                    case "Health Bar":
                        if (m_HealthBar == null)
                            m_HealthBar = child.gameObject.GetComponent<Image>();
                        if (m_HealthText == null)
                            m_HealthText = m_HealthBar.GetComponentInChildren<Text>();
                        break;

                    case "Negative Health Bar":
                        {
                            if (m_NegativeHealthBar == null)
                                m_NegativeHealthBar = child.gameObject.GetComponent<Image>();
                        }
                        break;
                }
            }

            Publisher.self.Subscribe(Event.FortressInitialized, OnInit);

            Publisher.self.Subscribe(Event.FortressHealthChanged, OnValueChanged);

            Publisher.self.Subscribe(Event.FortressDied, OnUnitDied);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void LateUpdate()
        {
            Vector3 worldPos = m_Parent.transform.position + m_Offset;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
        }

        private void SetHealth(Image a_Bar, float a_CurrentValue, float a_MaxValue)
        {
            m_HealthBar.fillAmount = a_CurrentValue / a_MaxValue;
            m_HealthText.text = a_CurrentValue + "/" + a_MaxValue;
        }
    
        private void OnValueChanged(Event a_Event, object[] a_Params)
        {
            IAttackable unit = a_Params[0] as IAttackable;

            if (unit == null || unit != m_Parent)
                return;

            SetHealth(m_HealthBar, unit.health, unit.maxHealth);

            if (m_NegativeHealthBar.fillAmount < m_HealthBar.fillAmount)
                m_NegativeHealthBar.fillAmount = m_HealthBar.fillAmount;

            m_LastHealthChange = 0;
            if (!m_HealthCoroutineIsRunning)
                StartCoroutine(ReduceNegativeHealthSpace());
        }

        private void OnInit(Event a_Event, params object[] a_Params)
        {
            IAttackable unit = a_Params[0] as IAttackable;

            if (unit == null || unit != m_Parent)
                return;

            SetHealth(m_HealthBar, unit.health, unit.maxHealth);
        }

        private void OnUnitDied(Event a_Event, object[] a_Params)
        {
            IAttackable unit = a_Params[0] as IAttackable;

            if (unit == null || unit != m_Parent || this == null)
                return;

            Publisher.self.UnSubscribe(Event.FortressInitialized, OnInit);

            Publisher.self.UnSubscribe(Event.FortressHealthChanged, OnValueChanged);

            Publisher.self.UnSubscribe(Event.FortressDied, OnUnitDied);

            Destroy(gameObject);
        }

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
    }
}
