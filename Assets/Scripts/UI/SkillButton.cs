using System;
using Interfaces;
using Library;
using UnityEngine;
using UnityEngine.UI;

using Event = Define.Event;

public class SkillButton : MonoBehaviour, IChildable<IUsesSkills>
{
    #region -- VARIABLES -- 
    [Header("Components")]
    [SerializeField]
    private Image m_Icon;
    [SerializeField]
    private Image m_UsageIndicator;
    [SerializeField]
    private Image m_CooldownIndicator;
    [SerializeField]
    private Text m_CooldownTimer;
    [SerializeField]
    private Text m_CostText;
    [SerializeField]
    private Button m_UpgradeButton;

    private IUsesSkills m_Parent;

    private Sprite m_Sprite;
    private int m_SkillIndex;

    #endregion

    #region -- PROPERTIES --
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

        m_UpgradeButton = GetComponentsInChildren<Button>()[1];
        Publisher.self.Subscribe(Event.SkillCooldownChanged, OnSkillCooldownChanged);
        Publisher.self.Subscribe(Event.UnitManaChanged, OnUnitManaChanged);
        Publisher.self.Subscribe(Event.UnitCanUpgradeSkill, OnCanUpgradeSkill);
        Publisher.self.Subscribe(Event.UpgradeSkill, OnUpgradeSkill);

        m_UpgradeButton.onClick.AddListener(OnSkillUpgradeClicked);
        m_UpgradeButton.gameObject.SetActive(false);
    }

    // Use this for initialization
    private void Start()
    {
        if (m_Icon == null)
            Debug.LogWarning(name + " has no icon image!");
        else
            m_Icon.sprite = m_Sprite;

        if (m_UsageIndicator == null)
            Debug.LogWarning(name + " has no usage indicator!");
        if (m_CooldownIndicator == null)
            Debug.LogWarning(name + " has no cooldown indicator!");

        if (m_CooldownTimer == null)
            Debug.LogWarning(name + " has no cooldown timer!");
        if (m_CostText == null)
            Debug.LogWarning(name + " has no cost text!");

        if (m_UpgradeButton == null)
            Debug.LogWarning(name + " has no upgrade button!");

        if (m_Parent.skills[m_SkillIndex].skillData.cost == 0)
            m_CostText.text = "";
        else
            m_CostText.text = string.Format("{0:0}", m_Parent.skills[m_SkillIndex].skillData.cost);

        m_CooldownTimer.text = "";

        SetIndicators();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnDestroy()
    {
        Publisher.self.UnSubscribe(Event.SkillCooldownChanged, OnSkillCooldownChanged);
        Publisher.self.UnSubscribe(Event.UnitManaChanged, OnUnitManaChanged);
        Publisher.self.UnSubscribe(Event.UnitCanUpgradeSkill, OnCanUpgradeSkill);
        Publisher.self.UnSubscribe(Event.UpgradeSkill, OnUpgradeSkill);
    }
    #endregion

    #region -- PRIVATE FUNCTIONS --
    private void OnClick()
    {
        Publisher.self.Broadcast(Event.UseSkill, m_Parent, m_SkillIndex);
    }

    private void SetIndicators()
    {
        m_UsageIndicator.enabled = true;

        if (m_Parent.skills[m_SkillIndex].level <= 0)
            m_UsageIndicator.color = new Color32(66, 66, 66, 225);
        else if (m_Parent.mana < m_Parent.skills[m_SkillIndex].skillData.cost)
            m_UsageIndicator.color = new Color32(79, 195, 247, 128);
        else
            m_UsageIndicator.enabled = false;
    }

    private void OnUnitManaChanged(Event a_Event, params object[] a_Params)
    {
        IStats unit = a_Params[0] as IStats;

        if (unit == null || unit != m_Parent)
            return;

        SetIndicators();
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
            m_CooldownIndicator.fillAmount = 0.0f;
        }
        else
        {
            parsedCooldown = string.Format("{0:0.0}", Math.Round(unit.skills[parsedSkillIndex].remainingCooldown, 1));
            m_CooldownIndicator.fillAmount =
                unit.skills[parsedSkillIndex].remainingCooldown / unit.skills[parsedSkillIndex].skillData.maxCooldown;
        }

        GetComponentInChildren<Text>().text = parsedCooldown;
    }

    private void OnCanUpgradeSkill(Event a_Event, params object[] a_Params)
    {
        IUsesSkills unit = a_Params[0] as IUsesSkills;

        if (unit == null || unit != m_Parent)
            return;

        m_UpgradeButton.gameObject.SetActive(true);
    }

    private void OnUpgradeSkill(Event a_Event, params object[] a_Params)
    {
        IUsesSkills unit = a_Params[0] as IUsesSkills;

        if (unit == null || unit != m_Parent)
            return;

        if (m_Parent.skills[m_SkillIndex].skillData.cost == 0)
            m_CostText.text = "";
        else
            m_CostText.text = string.Format("{0:0}", m_Parent.skills[m_SkillIndex].skillData.cost);

        m_UpgradeButton.gameObject.SetActive(false);
    }

    private void OnSkillUpgradeClicked()
    {
        m_UpgradeButton.gameObject.SetActive(false);
        Publisher.self.Broadcast(Event.UpgradeSkill, m_Parent, m_SkillIndex);

        SetIndicators();
    }

    #endregion
}
