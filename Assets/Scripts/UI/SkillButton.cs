using System;
using Interfaces;
using Library;
using UnityEngine;
using UnityEngine.UI;

using Event = Define.Event;

public class SkillButton : MonoBehaviour, IChildable<IUsesSkills>
{
    #region -- VARIABLES -- 
    [SerializeField]
    private Image m_CooldownIndicatorImage;

    [SerializeField]
    private IUsesSkills m_Parent;

    [SerializeField]
    private int m_SkillIndex;

    [SerializeField]
    private Sprite m_Sprite;

    private Button m_UpgradeSkillButton;

    #endregion

    #region -- PUBLIC Properties --
    public IUsesSkills parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        public int skillIndex
        {
            get { return m_SkillIndex; }
            set { m_SkillIndex = value; }
        }

        public Sprite sprite
        {
            get { return m_Sprite; }
            set { m_Sprite = value; }
        }
    #endregion

    #region -- UNITY FUNCTIONS --
    private void Awake()
    {
        if (GetComponent<Button>() != null)
            GetComponent<Button>().onClick.AddListener(OnClick);

        m_UpgradeSkillButton = GetComponentsInChildren<Button>()[1];
        Publisher.self.Subscribe(Event.SkillCooldownChanged, OnSkillCooldownChanged);
        Publisher.self.Subscribe(Event.UnitManaChanged, OnUnitManaChanged);
        Publisher.self.Subscribe(Event.UnitLevelUp, OnLevelUp);

        m_UpgradeSkillButton.onClick.AddListener(OnSkillUpgradeClicked);
        m_UpgradeSkillButton.gameObject.SetActive(false);
    }
    
    // Use this for initialization
    private void Start()
    {
        if (m_CooldownIndicatorImage == null)
            foreach (Transform child in transform)
            {
                if (child.tag == "Cooldown Indicator")
                    m_CooldownIndicatorImage = child.GetComponent<Image>();
            }

        GetComponentInChildren<Text>().text = "";
        GetComponent<Image>().sprite = m_Sprite;

        OnUnitManaChanged(Event.UnitManaChanged, parent);
    }

    // Update is called once per frame
    private void Update()
    {

    }
    #endregion

    #region -- PRIVATE FUNCTIONS --
    private void OnClick()
    {
        Publisher.self.Broadcast(Event.UseSkill, m_Parent, m_SkillIndex);
    }

    private void OnUnitManaChanged(Event a_Event, params object[] a_Params)
    {
        IStats unit = a_Params[0] as IStats;

        if (unit == null || unit != m_Parent)
            return;

        GetComponent<Image>().color = 
            unit.mana < m_Parent.skills[m_SkillIndex].skillData.cost ? 
                new Color32(33, 150, 243, 255) : 
                new Color32(255, 255, 255, 255);
    }

    private void OnSkillCooldownChanged(Event a_Event, params object[] a_Params)
    {
        IUsesSkills unit = a_Params[0] as IUsesSkills;
        int parsedSkillIndex = (int)a_Params[1];

        if (unit == null || unit != m_Parent || parsedSkillIndex != m_SkillIndex)
            return;

        string parsedCooldown;
        if (unit.skills[parsedSkillIndex].remainingCooldown == 0.0f)
        {
            parsedCooldown = "";
            m_CooldownIndicatorImage.fillAmount = 0.0f;
        }
        else
        {
            parsedCooldown = string.Format("{0:0.0}", Math.Round(unit.skills[parsedSkillIndex].remainingCooldown, 1));
            m_CooldownIndicatorImage.fillAmount = 
                unit.skills[parsedSkillIndex].remainingCooldown / unit.skills[parsedSkillIndex].skillData.maxCooldown;
        }

        GetComponentInChildren<Text>().text = parsedCooldown;
    }

    private void OnLevelUp(Event a_Event, params object[] a_Params)
    {
        IUsesSkills player = a_Params[0] as IUsesSkills;
        m_UpgradeSkillButton.gameObject.SetActive(true);

    }

    private void OnSkillUpgradeClicked()
    {
        Publisher.self.Broadcast(Event.UpgradeSkill, m_Parent, m_SkillIndex);
        m_UpgradeSkillButton.gameObject.SetActive(false);
    }

    #endregion
}
