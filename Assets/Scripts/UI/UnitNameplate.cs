using Unit;
using UnityEngine;
using UnityEngine.UI;

using Library;

using Event = Define.Event;

namespace UI
{
    public class UnitNameplate : MonoBehaviour, IParentable<GameObject>
    {
        [SerializeField]
        private GameObject m_Parent;

        [SerializeField]
        private Vector3 m_Offset;

        [SerializeField]
        private RectTransform m_HealthBar;
        [SerializeField]
        private RectTransform m_ManaBar;
        [SerializeField]
        private Text m_LevelText;

        private Vector2 m_HealthBarOriginalSize;
        private Vector2 m_ManaBarOriginalSize;

        public GameObject parent
        {
            get { return m_Parent; }
            set { m_Parent = value; Awake(); }
        }

        public void Awake()
        {
            if (m_Parent == null || m_Parent.GetComponent<IStats>() == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);

            GetComponents();

            Publisher.self.Subscribe(Event.UnitInitialized, OnInit);

            Publisher.self.Subscribe(Event.UnitHealthChanged, OnValueChanged);
            Publisher.self.Subscribe(Event.UnitManaChanged, OnValueChanged);
            Publisher.self.Subscribe(Event.UnitLevelChanged, OnValueChanged);
        }

        // Use this for initialization
        void Start()
        {

        }

        private void LateUpdate()
        {
            transform.position = m_Parent.transform.position + m_Offset;
        }

        private void GetComponents()
        {
            foreach (Transform child in transform)
            {
                switch (child.tag)
                {
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

            if (m_HealthBar != null)
                m_HealthBarOriginalSize = m_HealthBar.sizeDelta;
            if (m_ManaBar != null)
                m_ManaBarOriginalSize = m_ManaBar.sizeDelta;
        }

        private void OnInit(Event a_Event, params object[] a_Params)
        {
            IStats unit = a_Params[0] as IStats;

            if (unit == null ||
                unit != m_Parent.GetComponent<IStats>())
                return;

            SetBar(m_HealthBar, m_HealthBarOriginalSize, unit.health, unit.maxHealth);
            SetBar(m_ManaBar, m_ManaBarOriginalSize, unit.mana, unit.maxMana);
            SetLevel(unit.level);
        }
        private void OnValueChanged(Event a_Event, params object[] a_Params)
        {
            IStats unit = a_Params[0] as IStats;

            if (unit == null ||
                unit != m_Parent.GetComponent<IStats>())
                return;

            switch (a_Event)
            {
                case Event.UnitHealthChanged:
                    SetBar(m_HealthBar, m_HealthBarOriginalSize, unit.health, unit.maxHealth);
                    break;
                case Event.UnitManaChanged:
                    SetBar(m_ManaBar, m_ManaBarOriginalSize, unit.mana, unit.maxMana);
                    break;
                case Event.UnitLevelChanged:
                    SetLevel(unit.level);
                    break;
            }
        }

        private void SetBar(RectTransform a_Bar, Vector2 a_OriginalSize, float a_CurrentValue, float a_MaxValue)
        {
            if (a_Bar == null)
                return;

            float proportion = a_CurrentValue / a_MaxValue;

            a_Bar.sizeDelta = new Vector2(proportion * a_OriginalSize.x, a_Bar.sizeDelta.y);
            a_Bar.GetComponentInChildren<Text>().text = a_CurrentValue + "/" + a_MaxValue;
        }

        private void SetLevel(int a_Level)
        {
            if (m_LevelText == null)
                return;

            m_LevelText.text = "Level: " + a_Level;
        }
    }
}
