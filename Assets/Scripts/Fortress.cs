using UnityEngine;

//Currently Working on Fortress.
public class Fortress : MonoBehaviour
{
    private int m_Health;
    private int m_Defense;
    private string m_Name;
    //default constructor

    public Fortress(int a_Health, int a_Defense, string a_Name)
    {
        m_Health = a_Health;
        m_Defense = a_Defense;
        m_Name = a_Name;
    }

    //Health int property
    public int Health
    {
        get { return m_Health;}
        set { m_Health = value; }
    }
    //Defense int property
    public int Defense
    {
        get { return m_Defense; }
        set { m_Defense = value; }

    }
    //String name property
    public string Name
    {
        get { return m_Name; }
        set { m_Name = value; }
    }

}
