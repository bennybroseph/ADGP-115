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
        private RectTransform m_HealthBar;
        [SerializeField]
        private Text m_HealthText;

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

            foreach (Transform child in transform)
            {
                switch (child.gameObject.tag)
                {
                    case "Health Bar":
                        if (m_HealthBar == null)
                            m_HealthBar = child.gameObject.GetComponent<RectTransform>();
                        if (m_HealthText == null)
                            m_HealthText = m_HealthBar.GetComponentInChildren<Text>();
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
            transform.position = m_Parent.transform.position + m_Offset;
        }

        private void SetHealth(RectTransform a_Bar, float a_CurrentValue, float a_MaxValue)
        {
            m_HealthBar.GetComponent<Image>().fillAmount = a_CurrentValue / a_MaxValue;
            m_HealthText.text = a_CurrentValue + "/" + a_MaxValue;
        }
    
        private void OnValueChanged(Event a_Event, object[] a_Params)
        {
            IAttackable unit = a_Params[0] as IAttackable;

            if (unit == null || unit != m_Parent)
                return;

            SetHealth(m_HealthBar, unit.health, unit.maxHealth);
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

            Destroy(gameObject);
        }
    }
}
