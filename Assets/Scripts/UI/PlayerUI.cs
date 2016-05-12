using Library;
using Unit;
using UnityEngine;
using UnityEngine.UI;
using Event = Define.Event;

public class PlayerUI : MonoBehaviour
{

    [SerializeField]
    private RectTransform m_HealthBar;
    [SerializeField]
    private RectTransform m_ManaBar;
    [SerializeField]
    private Text m_HealthText;
    [SerializeField]
    private Text m_ManaText;
    [SerializeField]
    private Text m_LevelText;

    // Use this for initialization
    void Start()
    {
        if (m_HealthBar == null)
        {

        }
        if (m_ManaBar == null)
        {

        }

        if (m_HealthText == null)
        {

        }
        Publisher.self.Subscribe(Event.UnitHealthChanged, SetValues);
        Publisher.self.Subscribe(Event.UnitManaChanged, SetValues);
        Publisher.self.Subscribe(Event.UnitLevelChanged, SetValues);
    }

    public void SetValues(Event a_Event, params object[] a_Params)
    {
        IStats unit = a_Params[0] as IStats;

        switch (a_Event)
        {
            case Event.UnitHealthChanged:
                SetHealth(unit.health, unit.health);
                break;
            case Event.UnitManaChanged:
                SetMana(unit.mana, unit.mana);
                break;
            case Event.UnitLevelChanged:
                m_LevelText.text = "Level: " + unit.level.ToString();
                break;
        }
    }

    public void SetHealth(int a_CurrentHealth, int a_MaxHealth)
    {
        float value = (float)a_CurrentHealth / a_MaxHealth;

        m_HealthBar.localScale = new Vector3(value, m_HealthBar.localScale.y, m_HealthBar.localScale.z);
        m_HealthText.text = "Health: " + a_CurrentHealth + "/" + a_MaxHealth;
    }
    public void SetMana(int a_CurrentMana, int a_MaxMana)
    {
        float value = (float)a_CurrentMana / a_MaxMana;

        m_ManaBar.localScale = new Vector3(value, m_ManaBar.localScale.y, m_ManaBar.localScale.z);
        m_ManaText.text = "Mana: " + a_CurrentMana + "/" + a_MaxMana;
    }

}
