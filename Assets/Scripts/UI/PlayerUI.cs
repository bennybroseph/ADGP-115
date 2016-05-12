using UnityEngine;
using UnityEngine.UI;

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
    }

    public void SetHealth(int a_CurrentHealth, int a_MaxHealth)
    {
        float _value = (float)a_CurrentHealth / a_MaxHealth;

        m_HealthBar.localScale = new Vector3(_value, m_HealthBar.localScale.y, m_HealthBar.localScale.z);
        m_HealthText.text = "Health: " + a_CurrentHealth + "/" + a_MaxHealth;
    }

    public void SetMana(int a_CurrentMana, int a_MaxMana)
    {
        float _value = (float)a_CurrentMana / a_MaxMana;

        m_ManaBar.localScale = new Vector3(_value, m_ManaBar.localScale.y, m_ManaBar.localScale.z);
        m_ManaText.text = "Mana: " + a_CurrentMana + "/" + a_MaxMana;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
