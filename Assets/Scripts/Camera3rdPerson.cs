using Interfaces;
using Units.Controller;
using UnityEngine;

public class Camera3rdPerson : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Following;

    [SerializeField]
    private Vector3 m_Offset;

    [System.Serializable]
    private struct Box
    {
        public Vector3 m_Min;
        public Vector3 m_Max;
    }

    [SerializeField]
    private Box m_ScreenBorders;

    // Use this for initialization
    void Start()
    {
        if (m_Following == null)
            m_Following = UserController.self.controllables[0].gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, m_ScreenBorders.m_Min.x, m_ScreenBorders.m_Max.x),
            Mathf.Clamp(transform.position.y, m_ScreenBorders.m_Min.y, m_ScreenBorders.m_Max.y),
            Mathf.Clamp(transform.position.z, m_ScreenBorders.m_Min.z, m_ScreenBorders.m_Max.z));
    }
}
