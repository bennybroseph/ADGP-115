using UnityEngine;
using System.Collections;
using Interfaces;
using Units;

public class FloatingText : MonoBehaviour, IChildable<Unit>
{
    [SerializeField]
    private Vector3 m_Anchor;

    [SerializeField]
    private Unit m_Parent;

    public Vector3 anchor
    {
        get { return m_Anchor; }
        set { m_Anchor = value; }
    }

    public Unit parent
    {
        get { return m_Parent; }
        set { m_Parent = value; }
    }
    // Use this for initialization
    void Start()
    {
        transform.position = Camera.main.WorldToScreenPoint(
            m_Parent != null ? m_Parent.transform.position + m_Anchor : m_Anchor);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(
            m_Parent != null ? m_Parent.transform.position + m_Anchor : m_Anchor);
    }
}
