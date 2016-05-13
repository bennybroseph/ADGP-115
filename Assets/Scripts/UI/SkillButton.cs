using Library;
using UnityEngine;
using Unit;
using UnityEngine.UI;

using Event = Define.Event;

public class SkillButton : MonoBehaviour, IParentable<IUsesSkills>
{
    [SerializeField]
    private IUsesSkills m_Parent;

    [SerializeField]
    private int m_SkillIndex;

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

    private void Awake()
    {
        Publisher.self.Subscribe(Event.SkillCooldownChanged, OnSkillCooldownChanged);
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log(m_Parent);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnSkillCooldownChanged(Event a_Event, params object[] a_Params)
    {
        IUsesSkills skillUser  = a_Params[0] as IUsesSkills;
        int parsedSkillIndex = (int)a_Params[1];

        if (skillUser == null || skillUser != m_Parent || parsedSkillIndex != m_SkillIndex)
            return;

        string parsedCooldown;
        if (skillUser.skills[parsedSkillIndex].remainingCooldown == 0.0f)
        {
            parsedCooldown = "";
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            parsedCooldown = ((int)Mathf.Ceil(skillUser.skills[parsedSkillIndex].remainingCooldown)).ToString();
            GetComponent<Image>().color = Color.gray;
        }

        if (skillUser.mana < skillUser.skills[parsedSkillIndex].cost)
        {
            GetComponent<Image>().color = Color.blue;
        }

        GetComponentInChildren<Text>().text = parsedCooldown;
    }
}
