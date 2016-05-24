using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_Anchor;

    public Vector3 anchor
    {
        get { return m_Anchor; }
        set { m_Anchor = value; }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(m_Anchor);
    }
}
