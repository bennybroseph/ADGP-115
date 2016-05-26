using Library;
using Units.Controller;
using UnityEngine;

public class Camera3rdPerson : MonoSingleton<Camera3rdPerson>
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

    private Camera m_Camera;

    private void Start()
    {
        if (m_Following == null)
            m_Following = UserController.self.controllables[0].gameObject;

        if (m_Camera == null && GetComponentInChildren<Camera>() != null)
            m_Camera = GetComponentInChildren<Camera>();
        else
        {
            Debug.LogWarning(name + " needs a camera to be parented to this object!");
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Following == null)
            return;

        transform.position = m_Following.transform.position;
        m_Camera.transform.localPosition = m_Offset;

        //transform.eulerAngles = new Vector3(
        //    transform.eulerAngles.x, 
        //    Mathf.Lerp(transform.eulerAngles.y, m_Following.transform.eulerAngles.y, 0.5f * Time.deltaTime),
        //    transform.eulerAngles.z);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, m_ScreenBorders.m_Min.x, m_ScreenBorders.m_Max.x),
            Mathf.Clamp(transform.position.y, m_ScreenBorders.m_Min.y, m_ScreenBorders.m_Max.y),
            Mathf.Clamp(transform.position.z, m_ScreenBorders.m_Min.z, m_ScreenBorders.m_Max.z));
    }
}
